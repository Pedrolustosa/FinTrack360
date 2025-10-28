using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.SpendingByCategoryChart;

public class GetSpendingByCategoryChartQueryHandler(IAppDbContext context) : IRequestHandler<GetSpendingByCategoryChartQuery, List<CategorySpendingDto>>
{
    public async Task<List<CategorySpendingDto>> Handle(GetSpendingByCategoryChartQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var startDate = new DateTime(today.Year, today.Month, 1);

        var userAccountIds = await context.Accounts
            .Where(a => a.UserId == request.UserId)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var expenses = await context.Transactions
            .Where(t => userAccountIds.Contains(t.AccountId) &&
                        t.Type == TransactionType.Expense &&
                        t.Date >= startDate && t.Date <= today)
            .ToListAsync(cancellationToken);

        var totalSpent = expenses.Sum(e => e.Amount);
        if (totalSpent == 0) return [];

        var spendingGroups = expenses
            .Where(e => e.CategoryId.HasValue)
            .GroupBy(e => e.CategoryId!.Value);

        var categoryIds = spendingGroups.Select(g => g.Key);
        var categories = await context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var spendingByCategory = spendingGroups.Select(g =>
        {
            var totalAmount = g.Sum(e => e.Amount);
            return new CategorySpendingDto
            {
                CategoryName = categories.GetValueOrDefault(g.Key, "Uncategorized"),
                TotalAmount = totalAmount,
                Percentage = Math.Round((double)(totalAmount / totalSpent) * 100, 2)
            };
        }).ToList();

        return spendingByCategory.OrderByDescending(s => s.TotalAmount).ToList();
    }
}
