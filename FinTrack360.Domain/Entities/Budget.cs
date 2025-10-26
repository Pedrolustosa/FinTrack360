namespace FinTrack360.Domain.Entities;

public class Budget
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
