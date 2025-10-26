using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.GetFinancialGoals;

public class GetFinancialGoalsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetFinancialGoalsQuery, IEnumerable<FinancialGoal>>
{
    public async Task<IEnumerable<FinancialGoal>> Handle(GetFinancialGoalsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.FinancialGoals
            .AsNoTracking()
            .Where(fg => fg.UserId == userId)
            .OrderBy(fg => fg.TargetDate)
            .ToListAsync(cancellationToken);
    }
}
