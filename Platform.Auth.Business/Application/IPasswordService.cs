namespace Platform.Auth.Business.Application;

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}