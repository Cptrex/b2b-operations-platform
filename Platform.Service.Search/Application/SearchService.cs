using Platform.Logging.MongoDb;
using Platform.Logging.MongoDb.Contracts;
using Platform.Service.Search.Domain.Account;
using Platform.Service.Search.Domain.Business;
using Platform.Service.Search.Domain.User;
using Platform.Service.Search.Infrastructure.Logging;

namespace Platform.Service.Search.Application;

public class SearchService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoggingService _logging;

    public SearchService(IBusinessRepository businessRepository, IUserRepository userRepository, IAccountRepository accountRepository, ILoggingService logging)
    {
        _businessRepository = businessRepository;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _logging = logging;
    }

    public async Task<List<Domain.Business.Business>> SearchBusinessByNameAsync(string businessName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessName))
        {
            throw new ArgumentException("Business name cannot be empty", nameof(businessName));
        }

        await _logging.WriteAsync(LogType.Activity, LoggingAction.SearchBusinessByName, businessName, ct);

        return await _businessRepository.SearchByNameAsync(businessName, ct);
    }

    public async Task<List<User>> SearchUserByNameAsync(string userName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("User name cannot be empty", nameof(userName));
        }

        await _logging.WriteAsync(LogType.Activity, LoggingAction.SearchUserByName, userName, ct);

        return await _userRepository.SearchByUserNameAsync(userName, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByLoginAsync(string login, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cannot be empty", nameof(login));
        }

        await _logging.WriteAsync(LogType.Activity, LoggingAction.SearchAccountByLogin, login, ct);

        return await _accountRepository.SearchByLoginAsync(login, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByEmailAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        await _logging.WriteAsync(LogType.Activity, LoggingAction.SearchAccountByEmail, email, ct);

        return await _accountRepository.SearchByEmailAsync(email, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty", nameof(name));
        }

        await _logging.WriteAsync(LogType.Activity, LoggingAction.SearchAccountByName, name, ct);

        return await _accountRepository.SearchByNameAsync(name, ct);
    }
}