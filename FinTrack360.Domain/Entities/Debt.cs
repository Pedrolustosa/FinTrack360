using FinTrack360.Domain.Enums;

namespace FinTrack360.Domain.Entities;

public class Debt
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public DebtType Type { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal InterestRate { get; set; } // Annual Percentage Rate (APR)
    public decimal MinimumPayment { get; set; }
}
