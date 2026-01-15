using Platform.Auth.Business.Domain.Account.ValueObjects;

namespace Platform.Auth.Business.Domain.Account;

public class Account
{
    public int Id { get; private set;  }
    public string BusinessId { get; private set; }
    public string Login { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash Password { get; private set; }

    private Account() 
    {
        BusinessId = string.Empty;
        Login = string.Empty;
        Name = string.Empty;
        Email = null!;
        Password = null!;
    }

    public Account(string businessId, string login, string name, Email email, PasswordHash password)
    {
        BusinessId = businessId;
        Login = login;
        Name = name;
        Email = email;
        Password = password;
    }
}