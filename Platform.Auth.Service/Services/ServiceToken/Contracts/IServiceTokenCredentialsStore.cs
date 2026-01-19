using Platform.Shared.Results;

namespace Platform.Auth.Service.Services.ServiceToken.Contracts;

public interface IServiceCredentialStore
{
    Task<Result> ValidateAsync(string serviceId, string secret);
}
