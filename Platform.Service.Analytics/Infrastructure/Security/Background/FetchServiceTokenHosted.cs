using Platform.Shared.Abstractions.Contracts.Auth;

namespace Platform.Service.Analytics.Infrastructure.Security.Background;

public class FetchServiceTokenHosted : IHostedService
{
    private readonly IServiceTokenProvider _serviceTokenProvider;
    private readonly IAuthServiceTokenManager _tokenManager;

    public FetchServiceTokenHosted(IServiceTokenProvider serviceTokenProvider, IAuthServiceTokenManager tokenManager)
    {
        _serviceTokenProvider = serviceTokenProvider;
        _tokenManager = tokenManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {

            var token = await _serviceTokenProvider.GetTokenAsync();

            Console.WriteLine($"Fetched service token on start: {token}");

            var publicKey = _tokenManager.GetAuthServicePublicKey();

            if (!string.IsNullOrWhiteSpace(publicKey))
            {
                Console.WriteLine("Service public key available on start");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: failed to fetch service token on start: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
