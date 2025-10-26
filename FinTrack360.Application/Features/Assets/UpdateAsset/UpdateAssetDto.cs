namespace FinTrack360.Application.Features.Assets.UpdateAsset;

public record UpdateAssetDto(
    string Ticker,
    string Name,
    decimal Quantity,
    decimal AverageCost);
