using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Domain.User;

namespace Platform.Service.Search.Infrastructure.Db;

public class UserRepository : IUserRepository
{
    private readonly SearchContext _context;

    public UserRepository(SearchContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<List<User>> SearchByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await _context.Users.Where(u => u.UserName.Contains(userName)).ToListAsync(ct);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await GetByUserIdAsync(userId);

        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
