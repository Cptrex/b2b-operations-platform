using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Application;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Contracts.Events.Account;
using Platform.Shared.Messaging.Contracts.Events.Orders;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Platform.Service.Business.Infrastructure.Messaging;

public class BusinessRabbitMqConsumer : IRabbitMqMessageConsumer
{
    private readonly ConcurrentDictionary<string, Func<string, CancellationToken, Task>> _routingKeyHandlers = [];

    private readonly BusinessContext _businessContext;
    private readonly BusinessService _businessService;

    private readonly UserService _userService;

    public BusinessRabbitMqConsumer(BusinessContext businessContext, BusinessService businessService, UserService userService)
    {
        _businessContext = businessContext;
        _businessService = businessService;
        _userService = userService;

        _routingKeyHandlers = new()
        {
            ["auth.business.accountCreated"] = HandleAccountCreated,
            ["auth.business.accountDeleted"] = HandleAccountDeleted,
            ["orders.customerAddedToBusiness"] = HandleCustomerAddedToBusiness
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

    private async Task HandleAccountCreated(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<AccountCreatedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        var existingInboxMessage = await _businessContext.InboxMessages
            .FirstOrDefaultAsync(m => m.EventId == eventData.EventId && m.Consumer == "AccountCreated", ct);

        if (existingInboxMessage != null)
        {
            return;
        }

        await _businessContext.InboxMessages.AddAsync(new()
        {
            EventId = eventData.EventId,
            ProcessedAt = DateTime.UtcNow,
            Consumer = "AccountCreated"
        }, ct);

        await _businessContext.SaveChangesAsync(ct);
    }

    private async Task HandleAccountDeleted(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<AccountDeletedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        var existingInboxMessage = await _businessContext.InboxMessages
            .FirstOrDefaultAsync(m => m.EventId == eventData.EventId && m.Consumer == "AccountDeleted", ct);

        if (existingInboxMessage != null)
        {
            return;
        }

        var business = await _businessContext.Businesses
            .Include(b => b.Users)
            .Where(b => b.BusinessId == eventData.BusinessId)
            .FirstOrDefaultAsync(ct);

        if (business == null)
        {
            return;
        }

        var userToDelete = business.Users.FirstOrDefault(u => u.AccountId.ToString() == eventData.AccountId.ToString());

        if (userToDelete != null)
        {
            await _userService.DeleteUserAsync(userToDelete.Id, eventData.BusinessId, ct);
        }

        await _businessContext.InboxMessages.AddAsync(new()
        {
            EventId = eventData.EventId,
            ProcessedAt = DateTime.UtcNow,
            Consumer = "AccountDeleted"
        }, ct);

        await _businessContext.SaveChangesAsync(ct);
    }

    private async Task HandleCustomerAddedToBusiness(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<CustomerAddedToBusinessEvent>(json);

        if (eventData == null)
        {
            return;
        }

        var existingInboxMessage = await _businessContext.InboxMessages
            .FirstOrDefaultAsync(m => m.EventId == eventData.EventId && m.Consumer == "CustomerAddedToBusiness", ct);

        if (existingInboxMessage != null)
        {
            return;
        }

        await _businessService.AddCustomerAsync(
            eventData.CustomerId,
            eventData.BusinessId,
            eventData.CustomerName,
            eventData.CustomerEmail,
            eventData.CustomerPhone,
            ct);

        await _businessContext.InboxMessages.AddAsync(new()
        {
            EventId = eventData.EventId,
            ProcessedAt = DateTime.UtcNow,
            Consumer = "CustomerAddedToBusiness"
        }, ct);

        await _businessContext.SaveChangesAsync(ct);
    }
}
