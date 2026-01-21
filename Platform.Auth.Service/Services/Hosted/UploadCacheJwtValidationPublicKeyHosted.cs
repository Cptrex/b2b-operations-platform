using Paltform.Auth.Shared.JwtToken.Contracts;
using Platform.Shared.Cache.Contracts;
using Platform.Shared.Cache.Keys.Redis;

namespace Platform.Auth.Service.Services.Hosted;

public class UploadCacheJwtValidationPublicKeyHosted : IHostedService
{
    private readonly ICacheProvider _cache;
    private readonly IServiceTokenIssuer _token;

    public UploadCacheJwtValidationPublicKeyHosted(ICacheProvider cache, IServiceTokenIssuer token)
    {
        _cache = cache;
        _token = token;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = _token.GetPublicKey();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Service public key for jwt validation not found");
        }

        await _cache.SetAsync(AuthRedisKeys.JwtServicePublicKeyV1, token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
