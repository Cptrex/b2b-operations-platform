using Platform.Auth.Business.Domain.Account;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AccountRepository : IAccountRepository
{
    public async Task<Account?> CreateAsync(Account account)
    {
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId, string businessId)
    {
    }
}