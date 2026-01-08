using Platform.Service.Business.Application.Security;
using Polly;
using System.Net.Http.Headers;

namespace Platform.Service.Business.Infrastructure.Http;

public sealed class PollyDelegatingHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;
    private readonly IServiceTokenProvider _tokenProvider;
    public PollyDelegatingHandler(IServiceTokenProvider tokenProvider, IAsyncPolicy<HttpResponseMessage> policy)
    {
        _policy = policy;
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await _policy.ExecuteAsync( ct => base.SendAsync(request, ct), cancellationToken);
    }
}