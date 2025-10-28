namespace FinTrack360.Application.Features.Dashboard.UpcomingBills;

public class UpcomingBillDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}
