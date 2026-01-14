namespace Platform.Service.Business.Infrastructure.Db.Entity;

public class InboxMessage
{
    public Guid EventId { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }
    public string Consumer { get; set; } = string.Empty;
}