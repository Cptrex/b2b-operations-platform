namespace Platform.Shared.Messaging.Contracts.Events.Orders;

public sealed record OrderCreatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public Guid OrderId { get; init; }
    public string BusinessId { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; }
    public string CustomerEmail { get; init; }
    public string CustomerPhone { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public OrderCreatedEvent()
    {
        BusinessId = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CustomerPhone = string.Empty;
    }
}
