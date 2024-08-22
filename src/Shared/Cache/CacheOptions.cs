using Shared.Abstractions.Caching;

namespace Shared.Cache;

public class CacheOptions
{
    public string DefaultCacheType { get; set; } = nameof(CacheProviderType.InMemory);
    public double ExpirationTimeInMinute { get; set; } = 5;
    public string SerializationType { get; set; } = nameof(CacheSerializationType.Json);
    public RedisCacheOptions? RedisCacheOptions { get; set; } = default!;
    public InMemoryCacheOptions? InMemoryOptions { get; set; } = default!;
    public string DefaultCachePrefix { get; set; } = "Ch_";
}

public class RedisCacheOptions
{
    public string ConnectionString { get; set; } = default!;
}

public class InMemoryCacheOptions { }
