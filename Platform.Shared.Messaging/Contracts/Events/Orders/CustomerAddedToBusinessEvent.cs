namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record CustomerAddedToBusinessEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public string BusinessId { get; init; }
    public Guid CustomerId { get; init; }
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
