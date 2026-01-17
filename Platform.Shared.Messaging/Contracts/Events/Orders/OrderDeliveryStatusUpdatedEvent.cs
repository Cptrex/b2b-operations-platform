namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderDeliveryStatusUpdatedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid OrderId { get; init; }
    public string DeliveryStatus { get; init; }

    public OrderDeliveryStatusUpdatedEvent()
    {
        DeliveryStatus = string.Empty;
    }
}
