namespace FinTrack360.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty; // Categories are user-specific
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
