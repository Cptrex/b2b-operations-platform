using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paltform.Auth.Shared.JwtToken.Options;
using Paltform.Auth.Shared.JwtToken.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Paltform.Auth.Shared.JwtToken;

public sealed class RsaTokenIssuer : ITokenIssuer
{
    private readonly TokenOptions _options;
    private RSA? _rsa;
    private readonly string _publicKeyPem;

    public RsaTokenIssuer(IOptions<TokenOptions> options)
    {
        _options = options.Value;

        if (!File.Exists(_options.PrivateKeyPath))
        {
            throw new InvalidOperationException($"Private key file not found: {_options.PrivateKeyPath}");
        }
        if (!File.Exists(_options.PublicKeyPath))
        {
            throw new InvalidOperationException($"Public key file not found: {_options.PublicKeyPath}");
        }

        _publicKeyPem = File.ReadAllText(_options.PublicKeyPath);

        _rsa = RSA.Create();
        _rsa.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));
    }

    public string GetPublicKey() => _publicKeyPem;

    public IssuedToken ServiceIssue(string serviceId)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
        {
            throw new ArgumentException("serviceId is required", nameof(serviceId));
        }

        var issuer = _options.Issuer;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, serviceId),
            new Claim("type", "service")
        };

        var signingKey = new RsaSecurityKey(_rsa!);

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new IssuedToken(jwt, expiresAt);
    }

    public IssuedToken UserIssue(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("userId is required", nameof(userId));
        }
        var issuer = _options.Issuer;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiresMinutes);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("type", "user")
        };

        var signingKey = new RsaSecurityKey(_rsa!);

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new IssuedToken(jwt, expiresAt);
    }
}