using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Budgets.GetBudgetById;

public record GetBudgetByIdQuery(Guid Id) : IRequest<Budget?>;
