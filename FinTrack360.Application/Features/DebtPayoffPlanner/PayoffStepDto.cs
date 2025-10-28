namespace FinTrack360.Application.Features.DebtPayoffPlanner;

public class PayoffStepDto
{
    public int Month { get; set; }
    public Guid DebtId { get; set; }
    public string DebtName { get; set; } = string.Empty;
    public decimal PaymentAmount { get; set; }
    public decimal RemainingBalance { get; set; }
}
