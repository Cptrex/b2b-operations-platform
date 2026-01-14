namespace Platform.Auth.Business.Api.Dto;

public record CreateAccountDto(string BusinessId, string Login, string Name, string Email, string Password);
