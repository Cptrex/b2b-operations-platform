namespace Platform.Shared.Messaging.Contracts;

public interface IRabbitMqMessagePublisher
{
    Task PublishAsync(string routingKey, string body, CancellationToken ct = default);
}
