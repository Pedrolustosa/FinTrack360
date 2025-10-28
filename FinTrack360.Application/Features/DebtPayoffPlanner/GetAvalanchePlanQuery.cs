using MediatR;

namespace FinTrack360.Application.Features.DebtPayoffPlanner;

public record GetAvalanchePlanQuery(string UserId, decimal ExtraPayment) : IRequest<DebtPayoffPlanDto>;
