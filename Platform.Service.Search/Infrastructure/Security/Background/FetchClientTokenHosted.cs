using Platform.Shared.Cache.Contracts;
using Platform.Shared.Cache.Keys.Redis;

namespace Platform.Service.Search.Infrastructure.Security.Background;

public class FetchClientTokenHosted : IHostedService
{
    private readonly ICacheProvider _cache;
    private readonly IConfiguration _config;

    public FetchClientTokenHosted(ICacheProvider cache, IConfiguration config)
    {
        _cache = cache;
        _config = config;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var clientPublicKeyPath = _config["ClientJwt:PublicKeyPath"] ?? "auth_business_public.pem";

        try
        {
            var clientKey = await _cache.GetAsync(AuthRedisKeys.JwtClientPublicKeyV1);

            if (string.IsNullOrWhiteSpace(clientKey))
            {
                var authBusinessScheme = _config["AuthBusiness:Scheme"] ?? "http";
                var authBusinessHost = _config["AuthBusiness:Host"] ?? "localhost";
                var authBusinessPort = _config["AuthBusiness:Port"] ?? "80";

                var url = $"{authBusinessScheme}://{authBusinessHost}:{authBusinessPort}/api/v1/keys/client-public";

                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var resp = await http.GetAsync(url, cancellationToken);

                if (resp.IsSuccessStatusCode)
                {
                    clientKey = await resp.Content.ReadAsStringAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(clientKey))
                    {
                        try
                        {
                            await _cache.SetAsync(AuthRedisKeys.JwtClientPublicKeyV1, clientKey);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: failed to read client public key on start: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: failed to fetch client public key from Auth.Business: {resp.StatusCode}");
                }
            }

            if (!string.IsNullOrWhiteSpace(clientKey) && !File.Exists(clientPublicKeyPath))
            {
                File.WriteAllText(clientPublicKeyPath, clientKey);

                Console.WriteLine($"Fetched and saved Auth client public key to: {clientPublicKeyPath}");
            }
            else if (string.IsNullOrWhiteSpace(clientKey))
            {
                Console.WriteLine("Warning: client public key not found in cache or remote");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: failed to fetch client public key on start: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
