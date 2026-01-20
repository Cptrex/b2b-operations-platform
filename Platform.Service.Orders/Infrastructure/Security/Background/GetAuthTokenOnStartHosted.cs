using Platform.Shared.Abstractions.Contracts.Auth;

namespace Platform.Service.Orders.Infrastructure.Security.Background;

public class GetAuthTokenOnStartHosted : IHostedService
{
    private readonly IServiceTokenProvider _serviceTokenProvider;

    public GetAuthTokenOnStartHosted(IServiceTokenProvider provider)
    {
        _serviceTokenProvider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var result = await _serviceTokenProvider.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(result) == false)
        {
            Console.WriteLine($"Authorized for http service-to-service messaging: {result}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
