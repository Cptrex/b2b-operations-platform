namespace Platform.Shared.Messaging.Contracts;

public interface IRabbitMqMessageConsumer
{
    Task HandleAsync(string routingKey, string body, CancellationToken ct);
}