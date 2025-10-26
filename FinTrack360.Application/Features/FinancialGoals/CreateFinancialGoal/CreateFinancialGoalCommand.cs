using MediatR;

namespace FinTrack360.Application.Features.FinancialGoals.CreateFinancialGoal;

public record CreateFinancialGoalCommand(
    string Name,
    decimal TargetAmount,
    DateTime TargetDate) : IRequest<Guid>;
