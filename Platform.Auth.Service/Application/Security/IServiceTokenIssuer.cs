namespace Platform.Auth.Service.Application.Security;

public interface IServiceTokenIssuer
{
    IssuedToken Issue(string serviceId);
    string GetPublicKey();
}