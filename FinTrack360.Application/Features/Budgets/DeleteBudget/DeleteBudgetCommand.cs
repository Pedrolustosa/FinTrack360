using MediatR;

namespace FinTrack360.Application.Features.Budgets.DeleteBudget;

public record DeleteBudgetCommand(Guid Id) : IRequest<Unit>;
