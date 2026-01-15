namespace Platform.Service.Search.Domain.User;

public class User
{
    public int UserId { get; set; }
    public Guid AccountId { get; set; }
    public string UserName { get; set; }
    public long CreatedAt { get; set; }

    private User()
    {
        UserName = string.Empty;
    }

    public User(int userId, Guid accountId, string userName, long createdAt)
    {
        UserId = userId;
        AccountId = accountId;
        UserName = userName;
        CreatedAt = createdAt;
    }
}
