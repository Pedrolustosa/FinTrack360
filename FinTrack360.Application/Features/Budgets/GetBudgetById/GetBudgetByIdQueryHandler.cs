using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Budgets.GetBudgetById;

public class GetBudgetByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetBudgetByIdQuery, Budget?>
{
    public async Task<Budget?> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.Budgets
            .AsNoTracking()
            .Include(b => b.Category) // Include category details
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == userId, cancellationToken);
    }
}
