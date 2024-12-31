using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Caching;

namespace Shared.Cache;

public class CacheService : ICacheService
{
    private readonly HybridCache _hybridCache;
    private readonly CacheOptions _cacheOptions;

    public CacheService(HybridCache hybridCache, IOptions<CacheOptions> cacheOptions)
    {
        _hybridCache = hybridCache;
        _cacheOptions = cacheOptions.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await _hybridCache.GetOrCreateAsync<T>(
            key,
            ct => ValueTask.FromResult<T?>(default),
            cancellationToken: cancellationToken
        );
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string>? tags = null,
        TimeSpan? localExpiration = null,
        TimeSpan? distributedExpiration = null,
        CancellationToken cancellationToken = default
    )
    {
        // Use provided expirations or fallback to default from CacheOptions
        var options = new HybridCacheEntryOptions
        {
            LocalCacheExpiration =
                localExpiration ?? TimeSpan.FromMinutes(_cacheOptions.LocalCacheExpirationTimeInMinute),
            Expiration = distributedExpiration ?? TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute),
        };

        return await _hybridCache.GetOrCreateAsync(
            key: key,
            factory: factory,
            options: options,
            tags: tags,
            cancellationToken: cancellationToken
        );
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        IEnumerable<string>? tags = null,
        TimeSpan? localExpiration = null,
        TimeSpan? distributedExpiration = null,
        CancellationToken cancellationToken = default
    )
    {
        // Use provided expirations or fallback to default from CacheOptions
        var options = new HybridCacheEntryOptions
        {
            LocalCacheExpiration =
                localExpiration ?? TimeSpan.FromMinutes(_cacheOptions.LocalCacheExpirationTimeInMinute),
            Expiration = distributedExpiration ?? TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute),
        };

        await _hybridCache.SetAsync(
            key: key,
            value: value,
            options: options,
            tags: tags,
            cancellationToken: cancellationToken
        );
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _hybridCache.GetOrCreateAsync<object?>(
            key,
            ct => ValueTask.FromResult<object?>(null),
            cancellationToken: cancellationToken
        );

        return value != null;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveByTagAsync(tags, cancellationToken);
    }
}
