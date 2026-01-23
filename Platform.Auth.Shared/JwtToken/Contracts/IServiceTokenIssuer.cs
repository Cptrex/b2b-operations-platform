using Platform.Auth.Shared.JwtToken.Results;

namespace Platform.Auth.Shared.JwtToken.Contracts;

public interface IServiceTokenIssuer : ITokenIssuer
{
    IssuedToken ServiceIssue(string userId);
}
