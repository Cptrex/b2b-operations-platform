using Platform.Service.Business.Application.Security;
using Platform.Shared.Cache.Contracts;
using Platform.Shared.Cache.Keys.Redis;

namespace Platform.Service.Business.Infrastructure.Security;

public class AuthServiceTokenManager : IAuthServiceTokenManager
{
    private readonly string _authPublicKeyPath;
    private readonly ICacheProvider _cache;
    private string? _authPublicKey;

    public AuthServiceTokenManager(IConfiguration config, ICacheProvider cache)
    {
        _authPublicKeyPath = config["ServiceJwt:PublicKeyPath"] ?? "auth_business_public.pem";
        _cache = cache;

        LoadAuthServicePublicKey();
    }

    private void LoadAuthServicePublicKey()
    {
        if (File.Exists(_authPublicKeyPath))
        {
            _authPublicKey = File.ReadAllText(_authPublicKeyPath);
            Console.WriteLine($"Loaded Auth.Service public key from: {_authPublicKeyPath}");
        }
        else
        {
            _authPublicKey = _cache.GetAsync(AuthRedisKeys.JwtServicePublicKeyV1).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(_authPublicKey))
            {
                Console.WriteLine($"Warning: Auth.Service public key not found at {_authPublicKeyPath}");
            }
            else
            {
                File.WriteAllText(_authPublicKeyPath, _authPublicKey);
                Console.WriteLine($"Loaded Auth.Service public key from Redis and saved to: {_authPublicKeyPath}");
            }
        }
    }

    public void SaveAuthServicePublicKey(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            throw new ArgumentException("Public key cannot be empty", nameof(publicKey));
        }

        File.WriteAllText(_authPublicKeyPath, publicKey);

        _authPublicKey = publicKey;

        Console.WriteLine($"Saved Auth.Service public key to: {_authPublicKeyPath}");
    }

    public string? GetAuthServicePublicKey()
    {
        return _authPublicKey;
    }
}