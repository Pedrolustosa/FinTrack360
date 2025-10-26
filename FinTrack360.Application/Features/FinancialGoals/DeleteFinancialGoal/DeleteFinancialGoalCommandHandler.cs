using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.DeleteFinancialGoal;

public class DeleteFinancialGoalCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteFinancialGoalCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFinancialGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var financialGoal = await dbContext.FinancialGoals
            .FirstOrDefaultAsync(fg => fg.Id == request.Id && fg.UserId == userId, cancellationToken) ?? throw new Exception("Financial goal not found.");

        // Optional: Decide what to do with the contributed amount. For now, we just delete the goal.

        dbContext.FinancialGoals.Remove(financialGoal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
