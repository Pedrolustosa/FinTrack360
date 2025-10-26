using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.CreateFinancialGoal;

public class CreateFinancialGoalCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<CreateFinancialGoalCommand, Guid>
{
    public async Task<Guid> Handle(CreateFinancialGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");

        var financialGoal = new FinancialGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            TargetAmount = request.TargetAmount,
            CurrentAmount = 0, // Starts with 0
            TargetDate = request.TargetDate
        };

        dbContext.FinancialGoals.Add(financialGoal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return financialGoal.Id;
    }
}
