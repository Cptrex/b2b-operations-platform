namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderDeliveryStatusUpdatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string OrderId { get; init; }
    public string DeliveryStatus { get; init; }

    public OrderDeliveryStatusUpdatedEvent()
    {
        DeliveryStatus = string.Empty;
    }
}
