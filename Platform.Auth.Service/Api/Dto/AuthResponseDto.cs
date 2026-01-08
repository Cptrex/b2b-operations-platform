namespace Platform.Auth.Service.Api.Dto;

public sealed record AuthResponseDto(string Token, DateTimeOffset ExpiresAt, string PublicKey);