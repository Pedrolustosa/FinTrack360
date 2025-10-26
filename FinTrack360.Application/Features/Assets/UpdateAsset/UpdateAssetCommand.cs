using MediatR;

namespace FinTrack360.Application.Features.Assets.UpdateAsset;

public record UpdateAssetCommand(
    Guid AccountId,
    Guid Id,
    string Ticker,
    string Name,
    decimal Quantity,
    decimal AverageCost) : IRequest<Unit>;
