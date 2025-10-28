using FinTrack360.Domain.Enums;

namespace FinTrack360.Application.Features.Dashboard.AccountSummary;

public class AccountSummaryDto
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public AccountType Type { get; set; }
}
