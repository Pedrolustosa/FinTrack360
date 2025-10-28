using MediatR;

namespace FinTrack360.Application.Features.Dashboard.UpcomingBills;

public record GetUpcomingBillsQuery(string UserId) : IRequest<List<UpcomingBillDto>>;
