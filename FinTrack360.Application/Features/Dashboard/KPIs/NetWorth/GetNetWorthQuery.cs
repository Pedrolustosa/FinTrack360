using MediatR;

namespace FinTrack360.Application.Features.Dashboard.KPIs.NetWorth;

public record GetNetWorthQuery(string UserId) : IRequest<NetWorthDto>;
