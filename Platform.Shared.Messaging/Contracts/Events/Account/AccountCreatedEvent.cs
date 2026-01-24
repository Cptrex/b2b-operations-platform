namespace Platform.Shared.Messaging.Contracts.Events.Account;

public sealed record AccountCreatedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public int AccountId { get; init; }
    public string BusinessId { get; init; }
    public string Login { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public AccountCreatedEvent()
    {
        BusinessId = string.Empty;
        Login = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
    }
}
