namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record ProductRemovedFromCatalogEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string ProductId { get; init; } = Guid.NewGuid().ToString("D");
    public string BusinessId { get; init; }

    public ProductRemovedFromCatalogEvent()
    {
        BusinessId = string.Empty;
    }
}
