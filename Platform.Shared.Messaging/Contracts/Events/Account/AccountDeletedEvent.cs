namespace Platform.Shared.Messaging.Contracts.Events.Account;

public sealed record AccountDeletedEvent : IEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString("D");
    public DateTimeOffset OccuredAt { get; init; }

    public int AccountId { get; init; }
    public string BusinessId { get; init; }

    public AccountDeletedEvent()
    {
        BusinessId = string.Empty;
    }
}
