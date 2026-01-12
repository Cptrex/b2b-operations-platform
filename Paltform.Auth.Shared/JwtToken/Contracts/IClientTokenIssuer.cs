using Paltform.Auth.Shared.JwtToken.Results;

namespace Paltform.Auth.Shared.JwtToken.Contracts;

public interface IClientTokenIssuer : ITokenIssuer
{
    IssuedToken IssueAccessToken(string userId);
    IssuedToken IssueRefreshToken(string userId);
}