namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record ProductAvailabilityChangedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public Guid ProductId { get; init; }
    public string BusinessId { get; init; }
    public bool IsAvailable { get; init; }

    public ProductAvailabilityChangedEvent()
    {
        BusinessId = string.Empty;
    }
}
