namespace Platform.Shared.Abstractions.Contracts.Auth;

public sealed record ServiceTokenResult(string Token, DateTimeOffset ExpiresAt, string PublicKey);