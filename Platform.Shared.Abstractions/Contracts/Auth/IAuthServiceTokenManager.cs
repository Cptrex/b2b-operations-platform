namespace Platform.Shared.Abstractions.Contracts.Auth;

public interface IAuthServiceTokenManager
{
    void SaveAuthServicePublicKey(string publicKey);
    string? GetAuthServicePublicKey();
}