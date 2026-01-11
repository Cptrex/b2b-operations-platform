using System.Security.Cryptography;

namespace Paltform.Auth.Shared.Cryptography;

public static class RsaKeyPairGenerator
{
    public static void GenerateToken(string privateKeyPath, string publicKeyPath)
    {
        if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
        {
            using var rsa = RSA.Create(2048);
            var privateKey = rsa.ExportRSAPrivateKeyPem();
            var publicKey = rsa.ExportRSAPublicKeyPem();

            File.WriteAllText(privateKeyPath, privateKey);
            File.WriteAllText(publicKeyPath, publicKey);

            Console.WriteLine($"Generated RSA key pair: {privateKeyPath}, {publicKeyPath}");
        }
    }
}