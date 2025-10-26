using MediatR;

namespace FinTrack360.Application.Features.Assets.AddAsset;

public record AddAssetCommand(
    Guid AccountId,
    string Ticker,
    string Name,
    decimal Quantity,
    decimal AverageCost) : IRequest<Guid>;
