using Paltform.Auth.Shared.JwtToken.Contracts;
using Platform.Shared.Cache.Contracts;
using Platform.Shared.Cache.Keys.Redis;

namespace Platform.Auth.Business.Infrasturcture.Cache;

public class UploadCacheJwtValidationPublicKeyHosted : IHostedService
{
    private readonly ICacheProvider _cache;
    private readonly ITokenIssuer _token;

    public UploadCacheJwtValidationPublicKeyHosted(ICacheProvider cache, ITokenIssuer token)
    {
        _cache = cache;
        _token = token;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = _token.GetPublicKey();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Client public key for jwt validation not found");
        }

        await _cache.SetAsync(AuthRedisKeys.JwtClientPublicKeyV1, token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}