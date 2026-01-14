namespace Platform.Shared.Messaging.Contracts.Events;

internal interface IEvent
{
    Guid EventId { get; init; }
    DateTimeOffset OccuredAt { get; init; }
}