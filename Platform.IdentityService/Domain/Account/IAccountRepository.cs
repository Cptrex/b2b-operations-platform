namespace Platform.Auth.Business.Domain.Account;

public interface IAccountRepository
{
    Task<Account?> CreateAsync(Account account);
    Task<Account?> GetAccountByIdAsync(int accountId, string businessId);
}