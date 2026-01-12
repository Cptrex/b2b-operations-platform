using Paltform.Auth.Shared.JwtToken.Contracts;
using Platform.Auth.Business.Domain.Account;
using Platform.Shared.Results;
using Platform.Shared.Results.Enums;

namespace Platform.Auth.Business.Application;

public class AuthorizationService
{
    readonly IAccountRepository _accountRepository;
    readonly IClientTokenIssuer _tokenIssuer;
    readonly IPasswordService _passwordService;

    public AuthorizationService(IAccountRepository accountRepository, IClientTokenIssuer issuer, IPasswordService passwordService)
    {
        _accountRepository = accountRepository;
        _tokenIssuer = issuer;
        _passwordService = passwordService;
    }

    public async Task<Result<IssuedTokens>> Authorize(string login, string password, string businessId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cant be empty", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentException("Business id cant be empty", nameof(businessId));
        }

        var foundAccount = await _accountRepository.GetByLoginAsync(login, businessId, cancellationToken);

        if (foundAccount is null)
        {
            return Result<IssuedTokens>.Fail(new Error("Account with the provided login and business id was not found.", ResultErrorCategory.NotFound));
        }

        if (_passwordService.Verify(password, foundAccount.Password.Hash) == false)
        {
            return Result<IssuedTokens>.Fail(new Error("Login or password are incorrect", ResultErrorCategory.Unauthorized));
        }

        var issuedAccessToken = IssueUserToken(foundAccount);

        return Result<IssuedTokens>.Ok(issuedAccessToken);
    }

    IssuedTokens IssueUserToken(Account account)
    {
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var issuedAccessToken = _tokenIssuer.IssueAccessToken(account.Login);
        var issuedRefreshToken = _tokenIssuer.IssueRefreshToken(account.Login);

        return new IssuedTokens(issuedAccessToken, issuedRefreshToken);
    }
}