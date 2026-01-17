namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderConfirmedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid OrderId { get; init; }
    public DateTimeOffset ConfirmedAt { get; init; }
}
