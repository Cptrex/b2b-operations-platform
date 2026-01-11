namespace Paltform.Auth.Shared.JwtToken.Options;

public class TokenOptions
{
    public string Issuer { get; set; } = "auth.service";
    public string PrivateKeyPath { get; set; } = "service_private.pem";
    public string PublicKeyPath { get; set; } = "service_public.pem";
    public int ExpiresMinutes { get; set; } = 5;
}