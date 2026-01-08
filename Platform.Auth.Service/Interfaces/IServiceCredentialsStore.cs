namespace Platform.Auth.Service.Interfaces;

public interface IServiceCredentialStore
{
    Task<bool> ValidateAsync(string serviceId, string secret);
}
