using System.Collections.Concurrent;

namespace Platform.Shared.Messaging.Contracts;

public interface IRabbitMqMessageHandler
{
    Task HandleAsync(string routingKey, string body, CancellationToken ct);
}