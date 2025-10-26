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

namespace FinTrack360.Application.Features.Assets.GetAssets;

public class GetAssetsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAssetsQuery, IEnumerable<Asset>>
{
    public async Task<IEnumerable<Asset>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var accountExists = await dbContext.Accounts
            .AnyAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (!accountExists) throw new Exception("Account not found or access denied.");

        return await dbContext.Assets
            .Where(a => a.AccountId == request.AccountId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }
}
