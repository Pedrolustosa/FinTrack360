using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System; 
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack360.Infrastructure.Services;

public class RecurringTransactionService(IAppDbContext dbContext) : IRecurringTransactionService
{
    public async Task ProcessRecurringTransactionsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var recurringTransactions = await dbContext.RecurringTransactions
            .Where(rt => rt.StartDate.Date <= today && (!rt.EndDate.HasValue || rt.EndDate.Value.Date >= today))
            .ToListAsync();

        foreach (var rt in recurringTransactions)
        {
            if (ShouldTransactionBeCreated(rt, today))
            {
                var transactionExists = await dbContext.Transactions
                    .AnyAsync(t => t.Description == rt.Description && t.Amount == rt.Amount && t.Date.Date == today && t.AccountId == rt.AccountId);

                if (!transactionExists)
                {
                    var newTransaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = rt.AccountId,
                        CategoryId = rt.CategoryId,
                        Description = rt.Description,
                        Amount = rt.Amount,
                        Date = today,
                        Type = rt.Amount > 0 ? TransactionType.Income : TransactionType.Expense
                    };

                    var account = await dbContext.Accounts.FindAsync(rt.AccountId);
                    if (account != null)
                    {
                        account.CurrentBalance += newTransaction.Amount;
                    }

                    dbContext.Transactions.Add(newTransaction);
                }
            }
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);
    }

    private bool ShouldTransactionBeCreated(RecurringTransaction rt, DateTime today)
    {
        return rt.Frequency switch
        {
            Frequency.Weekly => today.DayOfWeek == rt.StartDate.DayOfWeek,
            Frequency.BiWeekly => (today.Date - rt.StartDate.Date).Days % 14 == 0,
            Frequency.Monthly => today.Day == rt.StartDate.Day,
            Frequency.Yearly => today.DayOfYear == rt.StartDate.DayOfYear,
            _ => false,
        };
    }
}
