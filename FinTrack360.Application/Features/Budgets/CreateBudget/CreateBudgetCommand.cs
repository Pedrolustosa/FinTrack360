using MediatR;

namespace FinTrack360.Application.Features.Budgets.CreateBudget;

public record CreateBudgetCommand(
    Guid CategoryId,
    decimal Amount,
    int Month,
    int Year) : IRequest<Guid>;
