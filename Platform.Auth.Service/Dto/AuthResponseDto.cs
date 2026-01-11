namespace Platform.Auth.Service.Dto;

public sealed record AuthResponseDto(string Token, DateTimeOffset ExpiresAt, string PublicKey);