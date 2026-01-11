namespace Paltform.Auth.Shared.JwtToken.Results;

public interface ITokenIssuer
{
    IssuedToken ServiceIssue(string serviceId);
    IssuedToken UserIssue(string userId);
    string GetPublicKey();
}