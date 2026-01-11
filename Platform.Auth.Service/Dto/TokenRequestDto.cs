namespace Platform.Auth.Service.Dto;

public sealed record TokenRequestDto(string ServiceId, string Secret);