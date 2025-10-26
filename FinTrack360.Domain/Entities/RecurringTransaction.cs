using FinTrack360.Domain.Enums;

namespace FinTrack360.Domain.Entities;

public class RecurringTransaction
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Frequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
