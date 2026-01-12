using Paltform.Auth.Shared.JwtToken.Results;
using Platform.Auth.Business.Domain.Account;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;

namespace Platform.Auth.Business.Application;

public class AuthorizationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenIssuer _tokenIssuer;

    public AuthorizationService(IAccountRepository accountRepository, ITokenIssuer issuer)
    {
        _accountRepository = accountRepository;
        _tokenIssuer = issuer;
    }

    public async Task<Result> TryAuthorize(string login, string password, string businessId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cant be empty", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentException("Business id cant be empty", nameof(businessId));
        }

        var foundAccount = await _accountRepository.GetByLoginAsync(login, password, businessId, cancellationToken);

        if (foundAccount is null)
        {
            return Result.Fail(new Error("Account with the provided login and business id was not found.", ResultErrorCategory.NotFound));
        }

        return Result.Ok();
    }

    public async Task<Result<IssuedToken>> IssueUserToken(Account account)
    {
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var issuedToken = _tokenIssuer.UserIssue(account.Login);

        return Result<IssuedToken>.Ok(issuedToken);
    }
}