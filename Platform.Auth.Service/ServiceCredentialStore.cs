using Platform.Auth.Service.Interfaces;

namespace Platform.Auth.Service;

public class ServiceCredentialStore : IServiceCredentialStore
{
    private readonly Dictionary<string, string> _services = [];

    public ServiceCredentialStore(IConfiguration config)
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

    public Task<bool> ValidateAsync(string serviceId, string secret)
    {
        if (string.IsNullOrWhiteSpace(serviceId) || !_services.TryGetValue(serviceId, out var storedSecret))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(storedSecret == secret);
    }
}