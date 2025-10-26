using FinTrack360.Domain.Enums;

namespace FinTrack360.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
