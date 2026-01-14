namespace Platform.Service.Business.Api.Dto;

public record CreateUserDto(string Login, string Password, string Email, string Name, string BusinessId);