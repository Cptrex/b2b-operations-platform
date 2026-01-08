namespace Platform.Auth.Business.Dto;

public class RefreshUserDto
{
    public string BusinessId { get; set; }
    public string Login { get; set; }
    public string RefreshToken { get; set; }
}