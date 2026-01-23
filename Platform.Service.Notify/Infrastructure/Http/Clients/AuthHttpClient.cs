using Platform.Service.Notify.Infrastructure.Http.Dtos;
using Platform.Shared.Abstractions.Contracts.Auth;
using Platform.Shared.Results;
using System.Text;
using System.Text.Json;

namespace Platform.Service.Notify.Infrastructure.Http.Clients;

public class AuthHttpClient : IAuthClient
{
    readonly HttpClient _httpClient;
    readonly IConfiguration _config;
    readonly IAuthServiceTokenManager _rsaKeyManager;

    public AuthHttpClient(HttpClient httpClient, IConfiguration config, IAuthServiceTokenManager rsaKeyManager)
    {
        _httpClient = httpClient;
        _config = config;
        _rsaKeyManager = rsaKeyManager;
    }

    public async Task<ServiceTokenResult> GetServiceTokenAsync()
    {
        var serviceId = _config["ServiceName"];
        var secret = _config["ServiceSecret"];

        if (string.IsNullOrWhiteSpace(serviceId) || string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("ServiceName or ServiceSecret not configured");
        }

        var content = new StringContent(
            JsonSerializer.Serialize(new
            {
                serviceId,
                secret
            }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("api/v1/internal/token", content);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Result<AuthTokenResponseDto>>();

        if (result is null)
        {
            throw new InvalidOperationException("dto is empty");
        }

        if (result.IsSuccess == false)
        {
            throw new InvalidOperationException("Get service token result failure");
        }

        _rsaKeyManager.SaveAuthServicePublicKey(result.Value.PublicKey);

        return new ServiceTokenResult(result.Value.Token, result.Value.ExpiresAt, result.Value.PublicKey);
    }
}
