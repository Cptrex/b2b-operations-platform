using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Platform.Service.Business.Infrastructure.Messaging;
using Platform.Shared.Messaging.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Platform.Shared.Messaging;

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _opt;
    private readonly IRabbitMqMessageHandler _handler;

    private IChannel? _channel;

    public RabbitMqConsumerHostedService(IRabbitMqMessageHandler handler, IConnection connection, IOptions<RabbitMqOptions> opt)
    {
        _connection = connection;
        _opt = opt.Value;
        _handler = handler;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _opt.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _opt.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        foreach (var key in _opt.BindingKeys.Distinct())
        {
            await _channel.QueueBindAsync(
                queue: _opt.QueueName,
                exchange: _opt.ExchangeName,
                routingKey: key,
                arguments: null,
                cancellationToken: cancellationToken);
        }

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: _opt.PrefetchCount,
            global: false,
            cancellationToken: cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var routingKey = ea.RoutingKey;
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                await _handler.HandleAsync(ea.RoutingKey, body, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _opt.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync(cancellationToken: cancellationToken);
                await _channel.DisposeAsync();
            }
        }
        catch { }

        await base.StopAsync(cancellationToken);
    }
}