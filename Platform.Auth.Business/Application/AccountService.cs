using Platform.Auth.Business.Domain.Account;
using Platform.Auth.Business.Domain.Account.ValueObjects;
using Platform.Auth.Business.Infrasturcture.Db;
using Platform.Auth.Business.Infrasturcture.Db.Entity;
using Platform.Shared.Messaging.Contracts.Events.Account;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;
using System.Text.Json;

namespace Platform.Auth.Business.Application;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordService _passwordService;
    private readonly AuthBusinessContext _context;

    public AccountService(IAccountRepository accountRepository, IPasswordService passwordService, AuthBusinessContext context)
    {
        _accountRepository = accountRepository;
        _passwordService = passwordService;
        _context = context;
    }

    public async Task<Result<Account>> CreateAccountAsync(string businessId, string login, string name, string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            return Result<Account>.Fail(new Error("BusinessId cannot be empty", ResultErrorCategory.Validation));
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            return Result<Account>.Fail(new Error("Login cannot be empty", ResultErrorCategory.Validation));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Account>.Fail(new Error("Name cannot be empty", ResultErrorCategory.Validation));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Account>.Fail(new Error("Email cannot be empty", ResultErrorCategory.Validation));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<Account>.Fail(new Error("Password cannot be empty", ResultErrorCategory.Validation));
        }

        var existingAccount = await _accountRepository.GetByLoginAsync(login, businessId, cancellationToken);

        if (existingAccount != null)
        {
            return Result<Account>.Fail(new Error("Account with this login already exists", ResultErrorCategory.Conflict));
        }

        var emailValueObject = Email.Create(email);
        var passwordHash = _passwordService.Hash(password);
        var passwordValueObject = PasswordHash.Create(passwordHash);

        var newAccount = new Account(businessId, login, name, emailValueObject, passwordValueObject);

        var createdAccount = await _accountRepository.CreateAsync(newAccount, cancellationToken);

        if (createdAccount == null)
        {
            return Result<Account>.Fail(new Error("Failed to create account", ResultErrorCategory.Conflict));
        }

        var accountCreatedEvent = new AccountCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            AccountId = createdAccount.Id,
            BusinessId = createdAccount.BusinessId,
            Login = createdAccount.Login,
            Name = createdAccount.Name,
            Email = createdAccount.Email.Value,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = accountCreatedEvent.EventId,
            Type = nameof(AccountCreatedEvent),
            RoutingKey = "auth.business.accountCreated",
            Payload = JsonSerializer.Serialize(accountCreatedEvent),
            OccurredAt = accountCreatedEvent.OccuredAt
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Account>.Ok(createdAccount);
    }

    public async Task<Result> DeleteAccountAsync(int accountId, string businessId, CancellationToken cancellationToken)
    {
        if (accountId <= 0)
        {
            return Result.Fail(new Error("Invalid account ID", ResultErrorCategory.Validation));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            return Result.Fail(new Error("BusinessId cannot be empty", ResultErrorCategory.Validation));
        }

        var account = await _accountRepository.GetByIdAsync(accountId, businessId, cancellationToken);
        
        if (account == null)
        {
            return Result.Fail(new Error("Account not found", ResultErrorCategory.NotFound));
        }

        await _accountRepository.DeleteAsync(accountId, businessId, cancellationToken);

        var accountDeletedEvent = new AccountDeletedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            AccountId = accountId,
            BusinessId = businessId
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = accountDeletedEvent.EventId,
            Type = nameof(AccountDeletedEvent),
            RoutingKey = "auth.business.accountDeleted",
            Payload = JsonSerializer.Serialize(accountDeletedEvent),
            OccurredAt = accountDeletedEvent.OccuredAt
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<List<Account>>> GetAllAccountsByBusinessIdAsync(string businessId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            return Result<List<Account>>.Fail(new Error("BusinessId cannot be empty", ResultErrorCategory.Validation));
        }

        var accounts = await _accountRepository.GetAllByBusinessIdAsync(businessId, cancellationToken);
        
        return Result<List<Account>>.Ok(accounts);
    }
}
