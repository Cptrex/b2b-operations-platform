using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;

namespace Platform.Service.Search.Infrastructure.Messaging;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);

    public OutboxPublisherBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<SearchContext>();
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

            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.LastError = ex.Message;
            }
        }

        if (unpublishedMessages.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
