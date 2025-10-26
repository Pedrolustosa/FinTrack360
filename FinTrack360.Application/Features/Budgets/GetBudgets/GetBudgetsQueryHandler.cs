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

namespace FinTrack360.Application.Features.Budgets.GetBudgets;

public class GetBudgetsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetBudgetsQuery, IEnumerable<Budget>>
{
    public async Task<IEnumerable<Budget>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var query = dbContext.Budgets
            .AsNoTracking()
            .Where(b => b.UserId == userId);

        if (request.Month.HasValue)
        {
            query = query.Where(b => b.Month == request.Month.Value);
        }

        if (request.Year.HasValue)
        {
            query = query.Where(b => b.Year == request.Year.Value);
        }

        var result = query.Include(b => b.Category);

        return await result.OrderBy(b => b.Year).ThenBy(b => b.Month).ToListAsync(cancellationToken);
    }
}
