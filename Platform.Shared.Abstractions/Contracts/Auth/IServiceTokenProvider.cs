namespace Platform.Shared.Abstractions.Contracts.Auth;

public interface IServiceTokenProvider
{
    Task<string> GetTokenAsync();
}