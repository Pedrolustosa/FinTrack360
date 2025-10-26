using MediatR;

namespace FinTrack360.Application.Features.Budgets.UpdateBudget;

public record UpdateBudgetCommand(Guid Id, decimal Amount) : IRequest<Unit>;
