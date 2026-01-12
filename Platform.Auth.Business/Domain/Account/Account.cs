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

    private Account() { }

    public Account(
        int id,
        string businessId,
        string login,
        string name,
        Email email,
        PasswordHash password)
    {
        Id = id;
        BusinessId = businessId;
        Login = login;
        Name = name;
        Email = email;
        Password = password;
    }
}