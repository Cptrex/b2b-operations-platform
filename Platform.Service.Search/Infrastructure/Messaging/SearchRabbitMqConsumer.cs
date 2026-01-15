using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Platform.Shared.Messaging.Contracts.Events.User;
using Platform.Shared.Messaging.Contracts.Events.Account;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Platform.Service.Search.Infrastructure.Messaging;

public class SearchRabbitMqConsumer : IRabbitMqMessageConsumer
{
    private readonly ConcurrentDictionary<string, Func<string, CancellationToken, Task>> _routingKeyHandlers = [];

    private readonly SearchContext _searchContext;

    public SearchRabbitMqConsumer(SearchContext searchContext)
    {
        _searchContext = searchContext;

        _routingKeyHandlers = new()
        {
            ["business.businessCreated"] = HandleBusinessCreated,
            ["auth.service.userCreated"] = HandleUserCreated,
            ["auth.business.accountCreated"] = HandleAccountCreated,
            ["auth.business.accountDeleted"] = HandleAccountDeleted
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

    private async Task HandleBusinessCreated(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<BusinessCreatedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        await _searchContext.InboxMessages.AddAsync(new()
        {
            EventId = eventData.EventId,
            Type = nameof(BusinessCreatedEvent),
            Payload = json,
            OccurredAt = eventData.OccuredAt
        }, ct);

        var existingBusiness = await _searchContext.Businesses.FirstOrDefaultAsync(b => b.BusinessId == eventData.BusinessId, ct);

        if (existingBusiness != null)
        {
            return;
        }

        var business = new Domain.Business.Business(
            eventData.BusinessId,
            eventData.BusinessName,
            eventData.CreatedAt.ToUnixTimeSeconds()
        );

        await _searchContext.Businesses.AddAsync(business, ct);
        await _searchContext.SaveChangesAsync(ct);
    }

    private async Task HandleUserCreated(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<UserCreatedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        await _searchContext.InboxMessages.AddAsync(new()
            {
                EventId = eventData.EventId,
                Type = nameof(UserCreatedEvent),
                Payload = json,
                OccurredAt = eventData.OccuredAt
            }, ct);

        var existingUser = await _searchContext.Users.FirstOrDefaultAsync(u => u.UserId == eventData.UserId, ct);

        if (existingUser != null)
        {
            return;
        }

        var user = new Domain.User.User(
            eventData.UserId,
            eventData.AccountId,
            eventData.UserName,
            eventData.CreatedAt.ToUnixTimeSeconds()
        );

        await _searchContext.Users.AddAsync(user, ct);
        await _searchContext.SaveChangesAsync(ct);
    }

    private async Task HandleAccountCreated(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<AccountCreatedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        await _searchContext.InboxMessages.AddAsync(new()
        {
            EventId = eventData.EventId,
            Type = nameof(AccountCreatedEvent),
            Payload = json,
            OccurredAt = eventData.OccuredAt
        }, ct);

        var existingAccount = await _searchContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == eventData.AccountId, ct);

        if (existingAccount != null)
        {
            return;
        }

        var account = new Domain.Account.Account(
            eventData.AccountId,
            eventData.BusinessId,
            eventData.Login,
            eventData.Name,
            eventData.Email,
            eventData.CreatedAt.ToUnixTimeSeconds()
        );

        await _searchContext.Accounts.AddAsync(account, ct);
        await _searchContext.SaveChangesAsync(ct);
    }

    private async Task HandleAccountDeleted(string json, CancellationToken ct)
    {
        var eventData = JsonSerializer.Deserialize<AccountDeletedEvent>(json);

        if (eventData == null)
        {
            return;
        }

        await _searchContext.InboxMessages.AddAsync(new()
            {
                EventId = eventData.EventId,
                Type = nameof(AccountDeletedEvent),
                Payload = json,
                OccurredAt = eventData.OccuredAt
            }, ct);

        var account = await _searchContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == eventData.AccountId, ct);

        if (account != null)
        {
            _searchContext.Accounts.Remove(account);
            await _searchContext.SaveChangesAsync(ct);
        }
    }
}
