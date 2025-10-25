namespace FinTrack360.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
