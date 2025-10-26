using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.GetFinancialGoalById;

public class GetFinancialGoalByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetFinancialGoalByIdQuery, FinancialGoal?>
{
    public async Task<FinancialGoal?> Handle(GetFinancialGoalByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.FinancialGoals
            .AsNoTracking()
            .FirstOrDefaultAsync(fg => fg.Id == request.Id && fg.UserId == userId, cancellationToken);
    }
}
