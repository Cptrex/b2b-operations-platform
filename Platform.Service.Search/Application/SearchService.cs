using Platform.Service.Search.Domain.Business;
using Platform.Service.Search.Domain.User;
using Platform.Service.Search.Domain.Account;

namespace Platform.Service.Search.Application;

public class SearchService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;

    public SearchService(IBusinessRepository businessRepository, IUserRepository userRepository, IAccountRepository accountRepository)
    {
        _businessRepository = businessRepository;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<Domain.Business.Business>> SearchBusinessByNameAsync(string businessName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessName))
        {
            throw new ArgumentException("Business name cannot be empty", nameof(businessName));
        }

        return await _businessRepository.SearchByNameAsync(businessName, ct);
    }

    public async Task<List<User>> SearchUserByNameAsync(string userName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("User name cannot be empty", nameof(userName));
        }

        return await _userRepository.SearchByUserNameAsync(userName, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByLoginAsync(string login, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cannot be empty", nameof(login));
        }

        return await _accountRepository.SearchByLoginAsync(login, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByEmailAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        return await _accountRepository.SearchByEmailAsync(email, ct);
    }

    public async Task<List<Domain.Account.Account>> SearchAccountByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty", nameof(name));
        }

        return await _accountRepository.SearchByNameAsync(name, ct);
    }
}
