namespace Platform.Service.Search.Domain.Account;

public interface IAccountRepository
{
    Task<Account?> GetByAccountIdAsync(int accountId);
    Task<List<Account>> SearchByLoginAsync(string login, CancellationToken ct = default);
    Task<List<Account>> SearchByEmailAsync(string email, CancellationToken ct = default);
    Task<List<Account>> SearchByNameAsync(string name, CancellationToken ct = default);
    Task AddAccountAsync(Account account);
    Task DeleteAccountAsync(int accountId);
    Task Save();
}
