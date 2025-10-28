using MediatR;

namespace FinTrack360.Application.Features.Dashboard.BudgetSummary;

public record GetBudgetSummaryQuery(string UserId) : IRequest<List<BudgetReportDto>>;
