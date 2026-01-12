using Paltform.Auth.Shared.JwtToken.Results;

namespace Platform.Auth.Business.Application;

public record IssuedTokens(IssuedToken AccessToken, IssuedToken RefreshToken);
