namespace Platform.Service.Business.Domain.User;

public class User
{
    public int Id { get; set; }
    public Guid AccountId { get; set; }
    public string UserName { get; set; }
    public long CreatedAt { get; set; }

    public string BusinessId { get; set; }
    public Business.Business Business { get; set; }

    private User()
    {
        UserName = string.Empty;
        BusinessId = string.Empty;
        Business = null!;
    }

    public User(string username, Guid accountId, string businessId)
    {
        UserName = username;
        AccountId = accountId;
        BusinessId = businessId;

        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Business = null!;
    }
}