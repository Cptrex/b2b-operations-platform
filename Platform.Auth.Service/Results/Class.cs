namespace Platform.Auth.Service.Results;

public sealed record IssuedToken(string Token, DateTimeOffset ExpiresAt);