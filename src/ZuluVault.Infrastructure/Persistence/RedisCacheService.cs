using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Infrastructure.Persistence;

/// <summary>
/// Redis cache service implementation
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(cachedValue))
            return null;

        return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value, _jsonOptions);
        var cacheOptions = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
            cacheOptions.AbsoluteExpirationRelativeToNow = expiration;
        else
            cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

        await _cache.SetStringAsync(key, serialized, cacheOptions, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }
}
