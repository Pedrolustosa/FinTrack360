using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Assets.GetAssetById;

public class GetAssetByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAssetByIdQuery, Asset?>
{
    public async Task<Asset?> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var account = await dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (account == null) return null; // Or throw an exception

        return await dbContext.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.AccountId == request.AccountId, cancellationToken);
    }
}
