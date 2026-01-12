using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paltform.Auth.Shared.JwtToken.Contracts;
using Paltform.Auth.Shared.JwtToken.Options;
using Paltform.Auth.Shared.JwtToken.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Paltform.Auth.Shared.JwtToken;

public sealed class RsaClientTokenIssuer : IClientTokenIssuer
{
    private readonly TokenOptions _options;
    private RSA? _rsa;
    private readonly string _publicKeyPem;

    public RsaClientTokenIssuer(IOptions<TokenOptions> options)
    {
        _options = options.Value;

        if (!File.Exists(_options.PrivateKeyPath))
        {
            throw new InvalidOperationException($"[Client] Private key file not found: {_options.PrivateKeyPath}");
        }
        if (!File.Exists(_options.PublicKeyPath))
        {
            throw new InvalidOperationException($"[Client] Public key file not found: {_options.PublicKeyPath}");
        }

        _publicKeyPem = File.ReadAllText(_options.PublicKeyPath);

        _rsa = RSA.Create();
        _rsa.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));
    }

    public string GetPublicKey() => _publicKeyPem;

    public IssuedToken IssueAccessToken(string userId)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiresAccessTokenMinutes);
        var token = UserIssue(userId, expiresAt);

        return token;
    }
    public IssuedToken IssueRefreshToken(string userId)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiresRefreshTokenMinutes);
        var token = UserIssue(userId, expiresAt);

        return token;
    }

    private IssuedToken UserIssue(string userId, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("userId is required", nameof(userId));
        }
        var issuer = _options.Issuer;
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