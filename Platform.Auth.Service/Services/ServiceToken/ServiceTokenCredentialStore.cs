using Platform.Auth.Service.Services.ServiceToken.Contracts;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;

namespace Platform.Auth.Service.Services.ServiceToken;

public class ServiceTokenCredentialStore : IServiceCredentialStore
{
    private readonly Dictionary<string, string> _services = [];

    public ServiceTokenCredentialStore(IConfiguration config)
    {
        _services = config
                   .GetSection("SERVICE_CREDENTIALS")
                   .GetChildren()
                   .ToDictionary(
                       x => x.Key,
                       x => x.Value!
                   );

        if (_services.Count == 0)
        {
            throw new InvalidOperationException("No service credentials configured");
        }
    }

    public Task<Result> ValidateAsync(string serviceId, string secret)
    {
        if (string.IsNullOrWhiteSpace(serviceId) || !_services.TryGetValue(serviceId, out var storedSecret))
        {
            return Task.FromResult(Result.Fail(new Error("Service Id cant be empty or not found", ResultErrorCategory.NotFound)));
        }

        if (storedSecret != secret)
        {
            return Task.FromResult(Result.Fail(new Error("Service not authorized for getting auth", ResultErrorCategory.Validation)));
        }

        return Task.FromResult(Result.Ok());
    }
}