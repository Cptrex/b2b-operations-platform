namespace Platform.Auth.Business.Application;

public class PasswordService : IPasswordService
{
    public string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException("Password cant be null or empy", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException("Password cant be null or empy", nameof(password));
        }
        if (string.IsNullOrEmpty(passwordHash))
        {
            throw new ArgumentNullException("PasswordHash cant be null or empy", nameof(passwordHash));
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
