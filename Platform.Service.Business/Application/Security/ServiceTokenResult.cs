namespace Platform.Service.Business.Application.Security.Dto;

public sealed record ServiceTokenResult(string Token, DateTimeOffset ExpiresAt, string PublicKey);