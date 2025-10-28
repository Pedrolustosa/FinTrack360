namespace FinTrack360.Application.Features.Dashboard.RecentTransactions;

public class RecentTransactionDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
}
