namespace Platform.Shared.Messaging.Contracts.Events.Account;

public sealed record AccountDeletedEvent : IEvent
{
    public Guid EventId { get; init; }
    public DateTimeOffset OccuredAt { get; init; }

    public int AccountId { get; init; }
    public string BusinessId { get; init; }

    public AccountDeletedEvent()
    {
        BusinessId = string.Empty;
    }
}
