namespace Platform.Shared.Messaging.Contracts.Events.User;

public sealed record UserCreatedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init;  } = DateTimeOffset.UtcNow;

    public int UserId { get; init; }
    public Guid AccountId { get; init; }
    public string UserName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }


    public UserCreatedEvent()
    {
        EventId = Guid.NewGuid();
        OccuredAt = DateTimeOffset.UtcNow;
    }
}