namespace Platform.Shared.Cache.Contracts;

public interface ICacheProvider
{
    Task SetAsync(string key, string value, TimeSpan? expire = null);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
}