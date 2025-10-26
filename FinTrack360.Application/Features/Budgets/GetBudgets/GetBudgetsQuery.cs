using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Budgets.GetBudgets;

public record GetBudgetsQuery(int? Month, int? Year) : IRequest<IEnumerable<Budget>>;
