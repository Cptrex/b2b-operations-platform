using Platform.Auth.Service.Results;

namespace Platform.Auth.Service.Interfaces;

public interface IServiceTokenIssuer
{
    IssuedToken Issue(string serviceId);
    string GetPublicKey();
}