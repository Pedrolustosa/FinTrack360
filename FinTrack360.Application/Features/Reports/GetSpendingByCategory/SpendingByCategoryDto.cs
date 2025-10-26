namespace FinTrack360.Application.Features.Reports.GetSpendingByCategory;

public record SpendingByCategoryDto
{
    public string CategoryName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}
