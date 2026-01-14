namespace Platform.Service.Business.Domain.Business;

public class Business
{
    public string BusinessId { get; set; }
    public string BusinessName { get; set; }
    public long CreatedAt { get; set; }

    private List<User.User> _users = [];
    public List<User.User> Users { get => _users; set => _users = value ?? []; }

    private Business() 
    {
        BusinessId = string.Empty;
        BusinessName = string.Empty;
        _users = [];
    }

    public Business(string businessId, string businessName)
    {
        BusinessId = businessId;
        BusinessName = businessName;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _users = [];
    }

    public void AddUser(User.User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Add(user);
    }

    public void RemoveUser(User.User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Remove(user);
    }
}