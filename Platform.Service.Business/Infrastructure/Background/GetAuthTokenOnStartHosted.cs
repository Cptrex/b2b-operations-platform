
using Platform.Service.Business.Application.Security;

namespace Platform.Service.Business.Infrastructure.Background;

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

        if (string.IsNullOrEmpty(result) == false)
        {
            Console.WriteLine($"Authorized for http service-to-service messaging: {result}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
