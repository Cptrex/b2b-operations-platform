using Microsoft.EntityFrameworkCore;
using Platform.Service.Orders.Infrastructure.Db;
using Platform.Service.Orders.Infrastructure.Db.Entity;
using Platform.Shared.Messaging.Contracts;
using System.Text.Json;

namespace Platform.Service.Orders.Infrastructure.Messaging;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqMessagePublisher _publisher;

    public OutboxPublisherBackgroundService(IServiceProvider serviceProvider, IRabbitMqMessagePublisher publisher)
    {
        _serviceProvider = serviceProvider;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (ct.IsCancellationRequested == false)
        {
            try
            {
                await ProcessOutboxMessagesAsync(ct);
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing outbox messages: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersContext>();

        var messages = await context.OutboxMessages
            .OrderBy(m => m.Id)
            .Take(10)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                await _publisher.PublishAsync(message.RoutingKey, message.Payload, ct);

                context.OutboxMessages.Remove(message);
                await context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish message {message.EventId}: {ex.Message}");
            }
        }
    }
}
