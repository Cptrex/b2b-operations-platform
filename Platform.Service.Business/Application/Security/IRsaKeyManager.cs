namespace Platform.Service.Business.Application.Security;

public interface IRsaKeyManager
{
    void SaveAuthServicePublicKey(string publicKey);
    string? GetAuthServicePublicKey();
}