using MediatR;

namespace FinTrack360.Application.Features.Reports.GetSpendingByCategory;

public record GetSpendingByCategoryQuery(DateTime StartDate, DateTime EndDate) : IRequest<IEnumerable<SpendingByCategoryDto>>;
