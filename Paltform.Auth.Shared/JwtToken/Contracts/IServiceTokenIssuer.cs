using Paltform.Auth.Shared.JwtToken.Results;

namespace Paltform.Auth.Shared.JwtToken.Contracts;

public interface IServiceTokenIssuer : ITokenIssuer
{
    IssuedToken ServiceIssue(string userId);
}
