namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record ProductAddedToCatalogEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string ProductId { get; init; } = Guid.NewGuid().ToString("D");
    public string BusinessId { get; init; }
    public string ProductName { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public bool IsAvailable { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public ProductAddedToCatalogEvent()
    {
        BusinessId = string.Empty;
        ProductName = string.Empty;
        Description = string.Empty;
    }
}
