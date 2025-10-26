using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Assets.AddAsset;

public class AddAssetCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<AddAssetCommand, Guid>
{
    public async Task<Guid> Handle(AddAssetCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");

        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");

        if (account.Type != AccountType.Investment) throw new Exception("Assets can only be added to investment accounts.");

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            Ticker = request.Ticker,
            Name = request.Name,
            Quantity = request.Quantity,
            AverageCost = request.AverageCost
        };

        dbContext.Assets.Add(asset);
        await dbContext.SaveChangesAsync(cancellationToken);

        return asset.Id;
    }
}
