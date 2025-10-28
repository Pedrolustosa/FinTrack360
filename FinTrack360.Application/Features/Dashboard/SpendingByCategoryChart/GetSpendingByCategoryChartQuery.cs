using MediatR;

namespace FinTrack360.Application.Features.Dashboard.SpendingByCategoryChart;

public record GetSpendingByCategoryChartQuery(string UserId) : IRequest<List<CategorySpendingDto>>;
