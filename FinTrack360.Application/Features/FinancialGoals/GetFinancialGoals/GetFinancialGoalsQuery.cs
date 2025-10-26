using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.FinancialGoals.GetFinancialGoals;

public record GetFinancialGoalsQuery : IRequest<IEnumerable<FinancialGoal>>;
