namespace FinTrack360.Application.Features.Dashboard.BudgetSummary;

public class BudgetReportDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsOverBudget { get; set; }
}
