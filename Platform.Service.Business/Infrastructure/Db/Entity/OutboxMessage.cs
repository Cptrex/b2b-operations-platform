namespace Platform.Service.Business.Infrastructure.Db.Entity;

public class OutboxMessage
{
    public int Id { get; set; }
    public Guid EventId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
}
