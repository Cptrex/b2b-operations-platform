namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderConfirmedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string OrderId { get; init; }
    public DateTimeOffset ConfirmedAt { get; init; }
}
