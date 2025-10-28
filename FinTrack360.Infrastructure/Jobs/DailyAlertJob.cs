using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack360.Infrastructure.Jobs;

public class DailyAlertJob : IDailyAlertJob
{
    private readonly IAppDbContext _context;

    public DailyAlertJob(IAppDbContext context)
    {
        _context = context;
    }

    public async Task RunAsync()
    {
        await CheckBudgetThresholds();
        await CheckRecurringTransactions();
        await CheckAccountBalances();

        await _context.SaveChangesAsync(default);
    }

    private async Task CheckBudgetThresholds()
    {
        // Logic to check budgets nearing their limit (e.g., > 90% spent)
        Console.WriteLine("Checking budget thresholds...");
        // Placeholder: find budgets over 90%
        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.Year == DateTime.UtcNow.Year && b.Month == DateTime.UtcNow.Month)
            .ToListAsync();

        foreach (var budget in budgets)
        {
            var startDate = new DateTime(budget.Year, budget.Month, 1);
            var endDate = startDate.AddMonths(1);

            var spentAmount = await _context.Transactions
                .Where(t => t.Account.UserId == budget.UserId &&
                            t.CategoryId == budget.CategoryId &&
                            t.Date >= startDate && t.Date < endDate &&
                            t.Type == Domain.Enums.TransactionType.Expense)
                .SumAsync(t => t.Amount);

            if (budget.Amount > 0 && (spentAmount / budget.Amount) > 0.9m && spentAmount < budget.Amount)
            {
                var percentage = (spentAmount / budget.Amount) * 100;
                var message = $"Alerta: Você atingiu {percentage:F0}% do seu orçamento de {budget.Category.Name}.";

                var notification = new Notification
                {
                    UserId = budget.UserId,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
            }
        }
    }

    private async Task CheckRecurringTransactions()
    {
        // Logic to check for upcoming recurring transactions (e.g., due in 3 days)
        Console.WriteLine("Checking recurring transactions...");
        var upcomingDate = DateTime.UtcNow.AddDays(3).Date;
        // TODO: Implement a robust mechanism to calculate NextDueDate based on Frequency.
        // For now, this part will be skipped.
        await Task.CompletedTask;
    }

    private async Task CheckAccountBalances()
    {
        // Logic to check for low account balances (e.g., < R$ 100)
        Console.WriteLine("Checking account balances...");
        const decimal minBalance = 100.0m;
        var lowBalanceAccounts = await _context.Accounts
            .Where(a => a.CurrentBalance < minBalance)
            .ToListAsync();

        foreach (var account in lowBalanceAccounts)
        {
            var message = $"Alerta: O saldo da sua conta '{account.Name}' está baixo: R$ {account.CurrentBalance:F2}.";
            var notification = new Notification
            {
                UserId = account.UserId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
        }
    }
}
