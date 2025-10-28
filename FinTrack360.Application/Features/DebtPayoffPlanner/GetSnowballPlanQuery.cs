using MediatR;

namespace FinTrack360.Application.Features.DebtPayoffPlanner;

public record GetSnowballPlanQuery(string UserId, decimal ExtraPayment) : IRequest<DebtPayoffPlanDto>;
