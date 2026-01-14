using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.User;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Contracts.Events.Business;
using Platform.Shared.Messaging.Contracts.Events.User;
using Microsoft.EntityFrameworkCore;

namespace Platform.Service.Business.Application;

public class BusinessService
{
    private readonly IRabbitMqMessagePublisher _eventPublisher;
    private readonly IBusinessRepository _businessRepository;
    private readonly BusinessContext _context;

    public BusinessService(IRabbitMqMessagePublisher eventPublisher, IBusinessRepository businessRepository, BusinessContext context)
    {
        _eventPublisher = eventPublisher;
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
        await _businessRepository.Save();

        var businessCreatedEvent = new BusinessCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = newBusiness.BusinessId,
            BusinessName = newBusiness.BusinessName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newBusiness.CreatedAt)
        };

        await _eventPublisher.PublishAsync("service.business.businessCreated", businessCreatedEvent, ct);

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
        await _businessRepository.Save();

        var businessDeletedEvent = new BusinessDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = businessId
        };

        await _eventPublisher.PublishAsync("service.business.businessDeleted", businessDeletedEvent, ct);
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
        await _businessRepository.Save();

        var userCreatedEvent = new UserCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = newUser.Id,
            AccountId = newUser.AccountId,
            UserName = newUser.UserName,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(newUser.CreatedAt)
        };

        await _eventPublisher.PublishAsync("business.user.created", userCreatedEvent, ct);

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

        await _businessRepository.Save();

        var userDeletedEvent = new UserDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            UserId = userId
        };

        await _eventPublisher.PublishAsync("service.business.userDeleted", userDeletedEvent, ct);
    }
}