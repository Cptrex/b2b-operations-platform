using Microsoft.Extensions.Options;
using Platform.Service.Business.Infrastructure.Messaging;
using Platform.Shared.Messaging.Contracts;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Platform.Shared.Messaging;

public sealed class RabbitMqPublisher : IRabbitMqMessagePublisher
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _opt;

    public RabbitMqPublisher(IConnection connection, IOptions<RabbitMqOptions> opt)
    {
        _connection = connection;
        _opt = opt.Value;
    }

    public async Task PublishAsync<T>(string routingKey, T body, CancellationToken ct = default)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await channel.ExchangeDeclareAsync(
            exchange: _opt.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        var json = body is string str ? str : JsonSerializer.Serialize(body);
        var bytes = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: _opt.ExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: bytes,
            cancellationToken: ct);
    }
}
