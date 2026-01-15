using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Domain.Account;

namespace Platform.Service.Search.Infrastructure.Db;

public class AccountRepository : IAccountRepository
{
    private readonly SearchContext _context;

    public AccountRepository(SearchContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByAccountIdAsync(int accountId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
    }

    public async Task<List<Account>> SearchByLoginAsync(string login, CancellationToken ct = default)
    {
        return await _context.Accounts.Where(a => a.Login.Contains(login)).ToListAsync(ct);
    }

    public async Task<List<Account>> SearchByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Accounts.Where(a => a.Email.Contains(email)).ToListAsync(ct);
    }

    public async Task<List<Account>> SearchByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Accounts.Where(a => a.Name.Contains(name)).ToListAsync(ct);
    }

    public async Task AddAccountAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public async Task DeleteAccountAsync(int accountId)
    {
        var account = await GetByAccountIdAsync(accountId);

        if (account != null)
        {
            _context.Accounts.Remove(account);
        }
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
