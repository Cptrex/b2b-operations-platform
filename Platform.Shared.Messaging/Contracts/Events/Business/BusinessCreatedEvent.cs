namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record BusinessCreatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string BusinessId { get; init; }
    public string BusinessName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public BusinessCreatedEvent()
    {
        BusinessId = string.Empty;
        BusinessName = string.Empty;
    }
}
