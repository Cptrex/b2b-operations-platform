using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.User;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Db.Entity;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Platform.Shared.Messaging.Contracts.Events.User;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Platform.Service.Business.Application;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly BusinessContext _context;

    public BusinessService(IBusinessRepository businessRepository, BusinessContext context)
    {
        _businessRepository = businessRepository;
        _context = context;
    }

    public async Task<Domain.Business.Business> CreateBusinessAsync(string businessId, string businessName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrWhiteSpace(businessName))
        {
            throw new ArgumentNullException(nameof(businessName));
        }

        var existingBusiness = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (existingBusiness is not null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' already exists");
        }

        var newBusiness = new Domain.Business.Business(businessId, businessName);

        await _businessRepository.CreateBusinessAsync(newBusiness);

        var businessCreatedEvent = new BusinessCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = newBusiness.BusinessId,
            BusinessName = newBusiness.BusinessName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newBusiness.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = businessCreatedEvent.EventId,
            Type = nameof(BusinessCreatedEvent),
            RoutingKey = "business.businessCreated",
            Payload = JsonSerializer.Serialize(businessCreatedEvent),
            OccurredAt = businessCreatedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        return newBusiness;
    }

    public async Task DeleteBusinessAsync(string businessId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        await _businessRepository.DeleteBusinessAsync(business);

        var businessDeletedEvent = new BusinessDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = businessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = businessDeletedEvent.EventId,
            Type = nameof(BusinessDeletedEvent),
            RoutingKey = "business.businessDeleted",
            Payload = JsonSerializer.Serialize(businessDeletedEvent),
            OccurredAt = businessDeletedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();
    }

    public async Task<User> CreateBusinessUserAsync(string username, Guid accountId, string businessId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("AccountId cannot be empty", nameof(accountId));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _businessRepository.GetByBusinessByIdAsync(businessId);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var newUser = new User(username, accountId, businessId);

        business.AddUser(newUser);

        await _businessRepository.UpdateBusinessAsync(business);

        var userCreatedEvent = new UserCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = newUser.Id,
            AccountId = newUser.AccountId,
            UserName = newUser.UserName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newUser.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = userCreatedEvent.EventId,
            Type = nameof(UserCreatedEvent),
            RoutingKey = "auth.service.userCreated",
            Payload = JsonSerializer.Serialize(userCreatedEvent),
            OccurredAt = userCreatedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();

        return newUser;
    }

    public async Task DeleteUserAsync(int userId, string businessId, CancellationToken ct)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("UserId must be greater than 0", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        var business = await _context.Businesses.Include(b => b.Users).FirstOrDefaultAsync(b => b.BusinessId == businessId, ct);

        if (business is null)
        {
            throw new InvalidOperationException($"Business with id '{businessId}' not found");
        }

        var user = business.Users.FirstOrDefault(u => u.Id == userId);

        if (user is null)
        {
            throw new InvalidOperationException($"User with id '{userId}' not found in business '{businessId}'");
        }

        business.RemoveUser(user);

        var userDeletedEvent = new UserDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = userId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = userDeletedEvent.EventId,
            Type = nameof(UserDeletedEvent),
            RoutingKey = "auth.service.userDeleted",
            Payload = JsonSerializer.Serialize(userDeletedEvent),
            OccurredAt = userDeletedEvent.OccuredAt
        }, ct);

        await _businessRepository.Save();
    }
}