using MediatR;

namespace FinTrack360.Application.Features.FinancialGoals.DeleteFinancialGoal;

public record DeleteFinancialGoalCommand(Guid Id) : IRequest<Unit>;
