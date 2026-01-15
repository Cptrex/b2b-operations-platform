namespace Platform.Service.Search.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByUserIdAsync(int userId);
    Task<List<User>> SearchByUserNameAsync(string userName, CancellationToken ct = default);
    Task AddUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task Save();
}
