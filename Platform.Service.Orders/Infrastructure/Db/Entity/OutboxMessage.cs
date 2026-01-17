namespace Platform.Service.Orders.Infrastructure.Db.Entity;

public class OutboxMessage
{
    public long Id { get; set; }
    public Guid EventId { get; set; }
    public string Type { get; set; }
    public string RoutingKey { get; set; }
    public string Payload { get; set; }
    public DateTimeOffset OccurredAt { get; set; }

    public OutboxMessage()
    {
        Type = string.Empty;
        RoutingKey = string.Empty;
        Payload = string.Empty;
    }
}
