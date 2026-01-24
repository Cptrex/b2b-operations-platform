namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record BusinessDeletedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public string BusinessId { get; init; }

    public BusinessDeletedEvent()
    {
        BusinessId = string.Empty;
    }
}
