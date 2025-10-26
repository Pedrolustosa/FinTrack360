using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.FinancialGoals.GetFinancialGoalById;

public record GetFinancialGoalByIdQuery(Guid Id) : IRequest<FinancialGoal?>;
