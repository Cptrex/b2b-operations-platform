namespace Platform.Shared.Messaging.Contracts.Events.User;

public sealed record UserCreatedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init;  }

    public int UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}