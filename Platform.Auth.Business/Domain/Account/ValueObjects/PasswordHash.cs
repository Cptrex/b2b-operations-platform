namespace Platform.Auth.Business.Domain.Account.ValueObjects;

public sealed class PasswordHash
{
    public string Hash { get; }

    private PasswordHash(string hash)
    {
        Hash = hash;
    }

    public static PasswordHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentNullException("Password hash cannot be null or empty.", nameof(hash));
        }

        return new PasswordHash(hash);
    }
}