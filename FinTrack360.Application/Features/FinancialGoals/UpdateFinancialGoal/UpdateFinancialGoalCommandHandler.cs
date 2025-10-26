using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.UpdateFinancialGoal;

public class UpdateFinancialGoalCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateFinancialGoalCommand, Unit>
{
    public async Task<Unit> Handle(UpdateFinancialGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var financialGoal = await dbContext.FinancialGoals
            .FirstOrDefaultAsync(fg => fg.Id == request.Id && fg.UserId == userId, cancellationToken) ?? throw new Exception("Financial goal not found.");

        financialGoal.Name = request.Name;
        financialGoal.TargetAmount = request.TargetAmount;
        financialGoal.TargetDate = request.TargetDate;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
