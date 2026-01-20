using Polly;

namespace Platform.Service.Search.Infrastructure.Http.Policies;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> BuildPolicy()
    {
        var timeout =
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(3));

        var retry =
            Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(2, i => TimeSpan.FromMilliseconds(100 * i));

        var breaker =
            Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => (int)r.StatusCode >= 500)
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(15));

        return Policy.WrapAsync(timeout, retry, breaker);
    }
}
