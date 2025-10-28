namespace FinTrack360.Application.Features.Dashboard.SpendingByCategoryChart;

public class CategorySpendingDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public double Percentage { get; set; }
}
