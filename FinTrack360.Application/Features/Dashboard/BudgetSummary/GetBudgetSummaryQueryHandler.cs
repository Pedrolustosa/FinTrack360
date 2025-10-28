using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.BudgetSummary;

public class GetBudgetSummaryQueryHandler(IAppDbContext context) : IRequestHandler<GetBudgetSummaryQuery, List<BudgetReportDto>>
{
    public async Task<List<BudgetReportDto>> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        var budgets = await context.Budgets
            .Where(b => b.UserId == request.UserId && b.Year == currentYear && b.Month == currentMonth)
            .Include(b => b.Category)
            .ToListAsync(cancellationToken);

        var budgetSummaries = new List<BudgetReportDto>();

        foreach (var budget in budgets)
        {
            var spentAmount = await context.Transactions
                .Where(t => t.CategoryId == budget.CategoryId &&
                            t.Date.Year == currentYear &&
                            t.Date.Month == currentMonth &&
                            t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount, cancellationToken);

            var remainingAmount = budget.Amount - spentAmount;

            budgetSummaries.Add(new BudgetReportDto
            {
                CategoryName = budget.Category.Name,
                BudgetAmount = budget.Amount,
                SpentAmount = spentAmount,
                RemainingAmount = remainingAmount,
                IsOverBudget = spentAmount > budget.Amount
            });
        }

        return budgetSummaries;
    }
}
