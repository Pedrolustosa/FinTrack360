using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Budgets.UpdateBudget;

public class UpdateBudgetCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateBudgetCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var budget = await dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == userId, cancellationToken) ?? throw new Exception("Budget not found.");

        budget.Amount = request.Amount;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
