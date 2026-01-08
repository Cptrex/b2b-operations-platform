namespace Platform.Service.Business.Application.Security;

public interface IServiceTokenProvider
{
    Task<string> GetTokenAsync();
}