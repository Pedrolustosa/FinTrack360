namespace FinTrack360.Application.Features.FinancialGoals.ContributeToFinancialGoal;

public record ContributeToFinancialGoalDto(
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount);
