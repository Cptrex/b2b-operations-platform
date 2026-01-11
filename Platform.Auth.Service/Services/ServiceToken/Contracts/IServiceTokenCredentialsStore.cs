namespace Platform.Auth.Service.Services.ServiceToken.Contracts;

public interface IServiceCredentialStore
{
    Task<bool> ValidateAsync(string serviceId, string secret);
}
