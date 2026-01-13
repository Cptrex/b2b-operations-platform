using Microsoft.Extensions.Caching.Distributed;
using Platform.Shared.Cache.Contracts;
using System.Text;

namespace Platform.Shared.Cache;

internal class RedisCacheProvider : ICacheProvider
{
    private readonly IDistributedCache _cache;

    public RedisCacheProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expire = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key is required", nameof(key));
        }

        var options = new DistributedCacheEntryOptions();

        if (expire is not null)
        {
            options.SetAbsoluteExpiration(expire.Value);
        }

        var bytes = Encoding.UTF8.GetBytes(value);

        await _cache.SetAsync(key, bytes, options);
    }

    public async Task<string?> GetAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key is required", nameof(key));
        }

        var bytes = await _cache.GetAsync(key);

        if (bytes is null)
        {
            return null;
        }

        return Encoding.UTF8.GetString(bytes);
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key is required", nameof(key));
        }

        await _cache.RemoveAsync(key);
    }
}