namespace Platform.Shared.Messaging.Contracts;

public interface IRabbitMqMessagePublisher
{
    Task PublishAsync<T>(string routingKey, T body, CancellationToken ct = default);
}
