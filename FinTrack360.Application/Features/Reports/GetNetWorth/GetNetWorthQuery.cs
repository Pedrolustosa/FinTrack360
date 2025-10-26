using MediatR;

namespace FinTrack360.Application.Features.Reports.GetNetWorth;

public record GetNetWorthQuery : IRequest<NetWorthDto>;
