namespace Platform.Service.Orders.Infrastructure.Db.Entity;

public class InboxMessage
{
    public long Id { get; set; }
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string Consumer { get; set; }

    public InboxMessage()
    {
        Consumer = string.Empty;
    }
}
