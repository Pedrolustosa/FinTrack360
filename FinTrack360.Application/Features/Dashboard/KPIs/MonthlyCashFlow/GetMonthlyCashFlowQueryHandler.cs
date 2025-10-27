using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.KPIs.MonthlyCashFlow;

public class GetMonthlyCashFlowQueryHandler(IAppDbContext context) : IRequestHandler<GetMonthlyCashFlowQuery, CashFlowDto>
{
    public async Task<CashFlowDto> Handle(GetMonthlyCashFlowQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var startDate = new DateTime(today.Year, today.Month, 1);
        var endDate = today;

        var userAccountsIds = await context.Accounts
            .Where(a => a.UserId == request.UserId)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var totalIncome = await context.Transactions
            .Where(t => userAccountsIds.Contains(t.AccountId) &&
                        t.Date >= startDate && t.Date <= endDate &&
                        t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount, cancellationToken);

        var totalExpenses = await context.Transactions
            .Where(t => userAccountsIds.Contains(t.AccountId) &&
                        t.Date >= startDate && t.Date <= endDate &&
                        t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);

        return new CashFlowDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetBalance = totalIncome - totalExpenses
        };
    }
}
