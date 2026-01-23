namespace Platform.Auth.Shared.JwtToken.Contracts;

public interface ITokenIssuer
{
    string GetPublicKey();
}