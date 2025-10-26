using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Assets.UpdateAsset;

public class UpdateAssetCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateAssetCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");

        var asset = await dbContext.Assets
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.AccountId == request.AccountId, cancellationToken) ?? throw new Exception("Asset not found.");

        asset.Ticker = request.Ticker;
        asset.Name = request.Name;
        asset.Quantity = request.Quantity;
        asset.AverageCost = request.AverageCost;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
