using MongoDB.Driver.Linq;
using Platform.Auth.Business.Domain.Account;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AccountRepository : IAccountRepository
{
    private readonly AuthBusinessContext _context;

    public AccountRepository(AuthBusinessContext context)
    {
        _context = context;
    }

    public async Task<Account?> CreateAsync(Account account)
    {
        var addedAccount = await _context.Accounts.AddAsync(account);

        await _context.SaveChangesAsync();

        return addedAccount.Entity;
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId, string businessId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(acc => acc.Id == accountId && acc.BusinessId == businessId);
    }
}