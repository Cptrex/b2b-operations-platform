namespace Platform.Auth.Business.Domain.Account.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException("email cant be null or empty");
        }

        return new Email(value);
    }

    private Email(string value)
    {
        Value = value;
    }
}
