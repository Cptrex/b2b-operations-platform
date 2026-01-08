namespace Platform.Auth.Service.Application.Security;

public interface IServiceCredentialStore
{
    Task<bool> ValidateAsync(string serviceId, string secret);
}
