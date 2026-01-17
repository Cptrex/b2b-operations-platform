namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderPaymentStatusUpdatedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid OrderId { get; init; }
    public string PaymentStatus { get; init; }

    public OrderPaymentStatusUpdatedEvent()
    {
        PaymentStatus = string.Empty;
    }
}
