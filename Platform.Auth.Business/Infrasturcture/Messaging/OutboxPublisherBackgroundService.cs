using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Auth.Business.Infrasturcture.Db;
using Platform.Shared.Messaging.Contracts;

namespace Platform.Auth.Business.Infrasturcture.Messaging;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);

    public OutboxPublisherBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OutboxPublisherBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox Publisher Background Service stopped");
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<AuthBusinessContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqMessagePublisher>();

        var unpublishedMessages = await context.OutboxMessages
            .Where(m => m.PublishedAt == null && m.RetryCount < 5)
            .OrderBy(m => m.OccurredAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var message in unpublishedMessages)
        {
            try
            {
                await publisher.PublishAsync(message.RoutingKey, message.Payload, cancellationToken);

                message.PublishedAt = DateTimeOffset.UtcNow;
                message.LastError = null;

                _logger.LogInformation("Successfully published outbox message {EventId} with routing key {RoutingKey}", 
                    message.EventId, message.RoutingKey);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.LastError = ex.Message;

                _logger.LogError(ex, "Failed to publish outbox message {EventId}. Retry count: {RetryCount}", 
                    message.EventId, message.RetryCount);
            }
        }

        if (unpublishedMessages.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
