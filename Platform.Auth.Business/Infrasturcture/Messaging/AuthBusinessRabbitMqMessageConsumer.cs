using Platform.Auth.Business.Infrasturcture.Db;
using Platform.Shared.Messaging.Contracts;
using System.Collections.Concurrent;

namespace Platform.Auth.Business.Infrasturcture.Messaging;

public class AuthBusinessRabbitMqMessageConsumer : IRabbitMqMessageConsumer
{
    private readonly ConcurrentDictionary<string, Func<string, CancellationToken, Task>> _routingKeyHandlers = [];

    private readonly AuthBusinessContext _context;

    public AuthBusinessRabbitMqMessageConsumer(AuthBusinessContext context)
    {
        _context = context;

        _routingKeyHandlers = new()
        {
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
}
