namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record CustomerAddedToBusinessEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string BusinessId { get; init; }
    public string CustomerId { get; init; } = Guid.NewGuid().ToString("D");
    public string CustomerName { get; init; }
    public string CustomerEmail { get; init; }
    public string CustomerPhone { get; init; }

    public CustomerAddedToBusinessEvent()
    {
        BusinessId = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CustomerPhone = string.Empty;
    }
}
