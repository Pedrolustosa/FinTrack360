using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.RecentTransactions;

public class GetRecentTransactionsQueryHandler(IAppDbContext context) : IRequestHandler<GetRecentTransactionsQuery, List<RecentTransactionDto>>
{
    public async Task<List<RecentTransactionDto>> Handle(GetRecentTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userAccountIds = await context.Accounts
            .Where(a => a.UserId == request.UserId)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var recentTransactions = await context.Transactions
            .Where(t => userAccountIds.Contains(t.AccountId))
            .OrderByDescending(t => t.Date)
            .Take(5)
            .Select(t => new RecentTransactionDto
            {
                Description = t.Description,
                Amount = t.Amount,
                Date = t.Date,
                CategoryName = t.Category != null ? t.Category.Name : "Uncategorized",
                AccountName = t.Account.Name
            })
            .ToListAsync(cancellationToken);

        return recentTransactions;
    }
}
