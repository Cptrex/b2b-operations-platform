using Microsoft.EntityFrameworkCore;
using Platform.Auth.Business.Domain.Account;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AccountRepository : IAccountRepository
{
    private readonly AuthBusinessContext _context;

    public AccountRepository(AuthBusinessContext context)
    {
        _context = context;
    }

    public async Task<Account?> CreateAsync(Account account, CancellationToken cancellationToken)
    {
        var addedAccount = await _context.Accounts.AddAsync(account, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return addedAccount.Entity;
    }

    public async Task<Account?> GetByIdAsync(int accountId, string businessId, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FirstOrDefaultAsync(acc => acc.Id == accountId && acc.BusinessId == businessId, cancellationToken);
    }

    public async Task<Account?> GetByLoginAsync(string login, string businessId, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FirstOrDefaultAsync(acc => acc.Login == login && acc.BusinessId == businessId, cancellationToken);
    }

    public async Task DeleteAsync(int accountId, string businessId, CancellationToken cancellationToken)
    {
        var account = await GetByIdAsync(accountId, businessId, cancellationToken);

        if (account != null)
        {
            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<Account>> GetAllByBusinessIdAsync(string businessId, CancellationToken cancellationToken)
    {
        return await _context.Accounts.Where(acc => acc.BusinessId == businessId).ToListAsync(cancellationToken);
    }
}