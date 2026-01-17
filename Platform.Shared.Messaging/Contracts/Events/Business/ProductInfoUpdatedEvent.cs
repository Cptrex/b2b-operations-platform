namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record ProductInfoUpdatedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid ProductId { get; init; }
    public string BusinessId { get; init; }
    public string ProductName { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public ProductInfoUpdatedEvent()
    {
        BusinessId = string.Empty;
        ProductName = string.Empty;
        Description = string.Empty;
    }
}
