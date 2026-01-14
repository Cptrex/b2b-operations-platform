using Platform.Service.Business.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Contracts.Events.User;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Platform.Service.Business.Infrastructure.Messaging;

public class BusinessRabbitMqHandler : IRabbitMqMessageHandler
{
    private readonly ConcurrentDictionary<string, Func<string, CancellationToken, Task>> _routingKeyHandlers = [];

    private readonly BusinessContext _businessContext;

    public BusinessRabbitMqHandler(BusinessContext businessContext)
    {
        _businessContext = businessContext;

        _routingKeyHandlers = new()
        {
            ["auth.business.userCreated"] = HandleUserCreated,
            ["auth.business.userUpdated"] = HandleUserUpdated,
            ["auth.business.userDeleted"] = HandleUserDeleted
        };
    }

    public async Task HandleAsync(string routingKey, string body, CancellationToken ct)
    {
        if (!_routingKeyHandlers.TryGetValue(routingKey, out var handler))
        {
            return;
        }

        await handler(body, ct);
    }

    private async Task HandleUserCreated(string json, CancellationToken ct)
    {
        var dto = JsonSerializer.Deserialize<UserCreatedEvent>(json);

        await _businessContext.InboxMessages.AddAsync(new()
        {
            EventId = Guid.NewGuid(),
            ProcessedAt = DateTime.UtcNow,
            Consumer = "UserCreated"
        }, ct);

        await _businessContext.SaveChangesAsync(ct);
    }
    private async Task HandleUserUpdated(string json, CancellationToken ct)
    {
    }
    private async Task HandleUserDeleted(string json, CancellationToken ct)
    {
    }

    Task IRabbitMqMessageHandler.HandleAsync(string routingKey, string body, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}