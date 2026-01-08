namespace Platform.Auth.Service.Application.Security;

public sealed record IssuedToken(string Token, DateTimeOffset ExpiresAt);