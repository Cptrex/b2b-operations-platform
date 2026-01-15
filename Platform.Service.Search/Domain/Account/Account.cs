namespace Platform.Service.Search.Domain.Account;

public class Account
{
    public int AccountId { get; set; }
    public string BusinessId { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public long CreatedAt { get; set; }

    private Account()
    {
        BusinessId = string.Empty;
        Login = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
    }

    public Account(int accountId, string businessId, string login, string name, string email, long createdAt)
    {
        AccountId = accountId;
        BusinessId = businessId;
        Login = login;
        Name = name;
        Email = email;
        CreatedAt = createdAt;
    }
}
