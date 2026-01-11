using Platform.Auth.Business.Domain.Account.ValueObjects;

namespace Platform.Auth.Business.Domain.Account;

public class Account
{
    public int Id { get; private set;  }
    public string BusinessId { get; private set; }
    public string Login { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string Password { get; private set; }

    internal Account(int id, string login, string name, string email, string password)
    {
        Id = id;
        Login = login;
        Name = name;
        Email = Email.Create(email);
        Password = password;
    }
}