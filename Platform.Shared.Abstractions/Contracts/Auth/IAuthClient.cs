namespace Platform.Shared.Abstractions.Contracts.Auth;

public interface IAuthClient
{
    Task<ServiceTokenResult> GetServiceTokenAsync();
}