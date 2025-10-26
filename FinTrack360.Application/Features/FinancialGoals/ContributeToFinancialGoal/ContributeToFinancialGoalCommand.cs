using MediatR;

namespace FinTrack360.Application.Features.FinancialGoals.ContributeToFinancialGoal;

public record ContributeToFinancialGoalCommand(
    Guid GoalId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount) : IRequest<Unit>;
