using Platform.Logging.MongoDb;
using Platform.Logging.MongoDb.Contracts;
using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Domain.User;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Db.Entity;
using Platform.Service.Business.Infrastructure.Logging;
using Platform.Shared.Messaging.Contracts.Events.User;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;
using System.Text.Json;

namespace Platform.Service.Business.Application;

public class UserService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly BusinessContext _context;
    private readonly ILoggingService _logging;

    public UserService(IBusinessRepository businessRepository, BusinessContext context, ILoggingService logging)
    {
        _businessRepository = businessRepository;
        _context = context;
        _logging = logging;
    }

    public async Task<Result<User>> CreateBusinessUserAsync(string username, Guid accountId, string businessId, CancellationToken ct)
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
            return Result<User>.Fail(new Error($"Business with id '{businessId}' not found", ResultErrorCategory.NotFound));
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

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessNewUserCreated, userCreatedEvent, ct);

        return Result< User>.Ok(newUser);
    }

    public async Task<Result> DeleteUserAsync(int userId, string businessId, CancellationToken ct)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("UserId must be greater than 0", nameof(userId));
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

        var user = business.Users.FirstOrDefault(u => u.Id == userId);

        if (user is null)
        {
            return Result.Fail(new Error($"User with id {userId} not found in business {businessId}", ResultErrorCategory.NotFound));
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

        await _logging.WriteAsync(LogType.Activity, LoggingAction.BusinessUserDeleted, userDeletedEvent, ct);

        return Result.Ok();
    }
}