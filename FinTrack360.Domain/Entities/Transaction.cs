using FinTrack360.Domain.Enums;

namespace FinTrack360.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid? CategoryId { get; set; } // Nullable for transfers or uncategorized
    public Category? Category { get; set; }
    public TransactionType Type { get; set; }
}
