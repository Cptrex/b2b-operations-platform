using Platform.Service.Business.Application.Security.Dto;

namespace Platform.Service.Business.Application;

public interface IAuthClient
{
    Task<ServiceTokenResult> GetServiceTokenAsync();
}