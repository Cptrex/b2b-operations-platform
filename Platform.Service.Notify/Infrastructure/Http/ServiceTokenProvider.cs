using Platform.Shared.Abstractions.Contracts.Auth;

namespace Platform.Service.Notify.Infrastructure.Http;

public sealed class ServiceTokenProvider : IServiceTokenProvider
{
    private readonly IAuthClient _authClient;
    private string? _token;
    private DateTimeOffset _expiresAt;

    public ServiceTokenProvider(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (_token != null && _expiresAt > DateTimeOffset.UtcNow.AddSeconds(30))
        {
            return _token;
        }

        var result = await _authClient.GetServiceTokenAsync();

        _token = result.Token;
        _expiresAt = result.ExpiresAt;

        return _token;
    }
}
