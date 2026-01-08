namespace Platform.Auth.Service.Api.Dto;

public sealed record TokenRequestDto(string ServiceId, string Secret);