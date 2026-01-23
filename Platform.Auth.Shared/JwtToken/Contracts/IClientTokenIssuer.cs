using Platform.Auth.Shared.JwtToken.Results;

namespace Platform.Auth.Shared.JwtToken.Contracts;

public interface IClientTokenIssuer : ITokenIssuer
{
    IssuedToken IssueAccessToken(string userId);
    IssuedToken IssueRefreshToken(string userId);
}