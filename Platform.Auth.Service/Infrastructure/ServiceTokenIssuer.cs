using Microsoft.IdentityModel.Tokens;
using Platform.Auth.Service.Application.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Platform.Auth.Service.Infrastructure;

public class ServiceTokenIssuer : IServiceTokenIssuer
{
    private readonly IConfiguration _config;
    private RSA? _rsa;
    private string? _publicKeyPem;

    public ServiceTokenIssuer(IConfiguration config)
    {
        _config = config;
        InitializeKeys();
    }

    private void InitializeKeys()
    {
        var privateKeyPath = _config["ServiceJwt:PrivateKeyPath"] ?? "jwt_private.pem";
        var publicKeyPath = _config["ServiceJwt:PublicKeyPath"] ?? "jwt_public.pem";

        if (!File.Exists(privateKeyPath))
        {
            throw new InvalidOperationException($"Private key file not found: {privateKeyPath}");
        }

        if (!File.Exists(publicKeyPath))
        {
            throw new InvalidOperationException($"Public key file not found: {publicKeyPath}");
        }

        var privateKeyPem = File.ReadAllText(privateKeyPath);
        _publicKeyPem = File.ReadAllText(publicKeyPath);

        _rsa = RSA.Create();
        _rsa.ImportFromPem(privateKeyPem);
    }

    public string GetPublicKey()
    {
        return _publicKeyPem ?? throw new InvalidOperationException("Public key not initialized");
    }

    public IssuedToken Issue(string serviceId)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
        {
            throw new ArgumentException("serviceId is required", nameof(serviceId));
        }

        var issuer = _config["ServiceJwt:Issuer"] ?? throw new InvalidOperationException("ServiceJwt:Issuer not configured");

        if (!int.TryParse(_config["ServiceJwt:ExpiresMinutes"], out var expiresMinutes))
        {
            throw new InvalidOperationException("ServiceJwt:ExpiresMinutes is invalid");
        }

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, serviceId),
            new Claim("type", "service")
        };

        var signingKey = new RsaSecurityKey(_rsa);

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.RsaSha256
            )
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new IssuedToken(jwt, expiresAt);
    }
}