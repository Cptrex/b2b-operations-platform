namespace Platform.Service.Business.Infrastructure.Security;

public interface IRsaKeyManager
{
    void SaveAuthServicePublicKey(string publicKey);
    string? GetAuthServicePublicKey();
}

public class RsaKeyManager : IRsaKeyManager
{
    private readonly string _authPublicKeyPath;
    private string? _authPublicKey;

    public RsaKeyManager(IConfiguration config)
    {
        _authPublicKeyPath = config["ServiceJwt:AuthServicePublicKeyPath"] ?? "auth_service_public.pem";
        LoadAuthServicePublicKey();
    }

    private void LoadAuthServicePublicKey()
    {
        if (File.Exists(_authPublicKeyPath))
        {
            _authPublicKey = File.ReadAllText(_authPublicKeyPath);
            Console.WriteLine($"Loaded Auth.Service public key from: {_authPublicKeyPath}");
        }
    }

    public void SaveAuthServicePublicKey(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            throw new ArgumentException("Public key cannot be empty", nameof(publicKey));
        }

        File.WriteAllText(_authPublicKeyPath, publicKey);
        _authPublicKey = publicKey;
        Console.WriteLine($"Saved Auth.Service public key to: {_authPublicKeyPath}");
    }

    public string? GetAuthServicePublicKey()
    {
        return _authPublicKey;
    }
}
