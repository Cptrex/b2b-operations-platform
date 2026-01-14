namespace Platform.Auth.Business.Domain.Account;

public interface IAccountRepository
{
    Task<Account?> CreateAsync(Account account, CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(int accountId, string businessId, CancellationToken cancellationToken);
    Task<Account?> GetByLoginAsync(string login, string businessId, CancellationToken cancellationToken);
    Task DeleteAsync(int accountId, string businessId, CancellationToken cancellationToken);
    Task<List<Account>> GetAllByBusinessIdAsync(string businessId, CancellationToken cancellationToken);
}