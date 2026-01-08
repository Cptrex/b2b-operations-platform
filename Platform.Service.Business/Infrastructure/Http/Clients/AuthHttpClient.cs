using Platform.Service.Business.Application;
using Platform.Service.Business.Application.Security;
using Platform.Service.Business.Infrastructure.Security;
using Platform.Service.Business.Application.Security.Dto;
using Platform.Service.Business.Infrastructure.Http.Dtos;
using System.Text;
using System.Text.Json;

namespace Platform.Service.Business.Infrastructure.Http.Clients;

public class AuthHttpClient : IAuthClient
{
    readonly HttpClient _httpClient;
    readonly IConfiguration _config;
    readonly IRsaKeyManager _rsaKeyManager;

    public AuthHttpClient(HttpClient httpClient, IConfiguration config, IRsaKeyManager rsaKeyManager)
    {
        _httpClient = httpClient;
        _config = config;
        _rsaKeyManager = rsaKeyManager;
    }

    public async Task<ServiceTokenResult> IssueServiceTokenAsync()
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

        var dto = await response.Content.ReadFromJsonAsync<AuthTokenResponseDto>() ?? throw new InvalidOperationException("Empty auth response");

        // Сохраняем публичный ключ Auth.Service
        _rsaKeyManager.SaveAuthServicePublicKey(dto.PublicKey);

        return new ServiceTokenResult(dto.Token, dto.ExpiresAt, dto.PublicKey);
    }
}