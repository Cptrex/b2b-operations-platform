namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderPaymentStatusUpdatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string OrderId { get; init; }
    public string PaymentStatus { get; init; }

    public OrderPaymentStatusUpdatedEvent()
    {
        PaymentStatus = string.Empty;
    }
}
