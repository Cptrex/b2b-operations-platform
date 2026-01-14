namespace Platform.Service.Business.Application.Security;

public interface IAuthServiceTokenManager
{
    void SaveAuthServicePublicKey(string publicKey);
    string? GetAuthServicePublicKey();
}