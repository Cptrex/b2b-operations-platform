namespace Paltform.Auth.Shared.JwtToken.Results;

public sealed record IssuedToken(string Token, DateTimeOffset ExpiresAt);