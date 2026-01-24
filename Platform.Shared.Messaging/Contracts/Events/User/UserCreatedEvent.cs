namespace Platform.Shared.Messaging.Contracts.Events.User;

public sealed record UserCreatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init;  } = DateTimeOffset.UtcNow;

    public int UserId { get; init; }
    public string AccountId { get; init; } = Guid.NewGuid().ToString("D");
    public string UserName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }


    public UserCreatedEvent()
    {
        EventId = Guid.NewGuid().ToString("D");
        OccuredAt = DateTimeOffset.UtcNow;
    }
}