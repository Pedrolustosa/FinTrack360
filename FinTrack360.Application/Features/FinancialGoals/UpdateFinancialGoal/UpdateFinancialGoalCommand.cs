using MediatR;

namespace FinTrack360.Application.Features.FinancialGoals.UpdateFinancialGoal;

public record UpdateFinancialGoalCommand(
    Guid Id,
    string Name,
    decimal TargetAmount,
    DateTime TargetDate) : IRequest<Unit>;
