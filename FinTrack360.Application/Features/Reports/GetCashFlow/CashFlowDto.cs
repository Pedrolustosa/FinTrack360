namespace FinTrack360.Application.Features.Reports.GetCashFlow;

public record CashFlowDto
{
    public string Month { get; init; } = string.Empty;
    public decimal TotalIncome { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal NetFlow => TotalIncome - TotalExpense;
}
