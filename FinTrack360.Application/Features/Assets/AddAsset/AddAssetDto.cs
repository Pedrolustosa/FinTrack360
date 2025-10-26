namespace FinTrack360.Application.Features.Assets.AddAsset;

public record AddAssetDto(
    string Ticker,
    string Name,
    decimal Quantity,
    decimal AverageCost);
