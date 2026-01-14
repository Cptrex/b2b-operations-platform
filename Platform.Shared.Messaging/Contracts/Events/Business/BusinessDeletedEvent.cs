namespace Platform.Shared.Messaging.Contracts.Events.Business;

public sealed record BusinessDeletedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public string BusinessId { get; init; }

    public BusinessDeletedEvent()
    {
        BusinessId = string.Empty;
    }
}
