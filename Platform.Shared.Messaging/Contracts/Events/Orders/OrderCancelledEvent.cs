namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderCancelledEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid OrderId { get; init; }
    public DateTimeOffset CancelledAt { get; init; }
}
