namespace Platform.Service.Analytics.Infrastructure.Http.Dtos;

public class AuthTokenResponseDto
{
    public string Token { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string PublicKey { get; set; }
}
