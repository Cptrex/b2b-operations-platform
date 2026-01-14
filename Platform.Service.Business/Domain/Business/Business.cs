namespace Platform.Service.Business.Domain.Business;

public class Business
{
    public int Id { get; set;  }
    public string BusinessId { get; set; }
    public string BusinessName { get; set; }
    public long CreatedAt { get; set; }
    public List<User.User> Users { get; set; }

    public Business(string businessId, string businessName)
    {
        BusinessId = businessId;
        BusinessName = businessName;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Users = [];
    }

    public void AddUser(User.User user)
    {
        Users.Add(user);
    }

    public void RemoveUser(User.User user)
    {
        Users.Remove(user);
    }
}