namespace Platform.Service.Business.Domain.Business;

public class Business
{
    public int Id { get; set;  }
    public string BusinessKey { get; set; }
    public string BusinessName { get; set; }
    public long CreatedAt { get; set; }
    public List<User.User> Users { get; set; }

    internal Business(string businessKey, string businessName)
    {
        BusinessKey = businessKey;
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