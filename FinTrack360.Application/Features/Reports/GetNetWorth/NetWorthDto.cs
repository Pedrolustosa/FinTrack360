namespace FinTrack360.Application.Features.Reports.GetNetWorth;

public record NetWorthDto
{
    public decimal TotalAccountBalance { get; init; }
    public decimal TotalAssetValue { get; init; }
    public decimal NetWorth => TotalAccountBalance + TotalAssetValue;
}
