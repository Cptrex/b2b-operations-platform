namespace Platform.Shared.Messaging.Contracts.Events.User;

public sealed record UserDeletedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public int UserId { get; init; }
}
