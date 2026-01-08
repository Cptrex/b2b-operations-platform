namespace Platform.Auth.Service.Dto;

public class AuthResponseDto
{
    public string Token { get; set;  }
    public DateTimeOffset ExpiresAt { get; set; }
    public string PublicKey { get; set; }

    public AuthResponseDto() { }

    public AuthResponseDto(string token, DateTimeOffset expiresAt, string publicKey)
    {
        Token = token;
        ExpiresAt = expiresAt;
        PublicKey = publicKey;
    }
}