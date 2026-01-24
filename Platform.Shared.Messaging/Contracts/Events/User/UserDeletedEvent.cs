namespace Platform.Shared.Messaging.Contracts.Events.User;

public sealed record UserDeletedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public int UserId { get; init; }
}
