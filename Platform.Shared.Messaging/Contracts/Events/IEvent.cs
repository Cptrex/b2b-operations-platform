namespace Platform.Shared.Messaging.Contracts.Events;

internal interface IEvent
{
    string EventId { get; init; }
    DateTimeOffset OccuredAt { get; init; }
}