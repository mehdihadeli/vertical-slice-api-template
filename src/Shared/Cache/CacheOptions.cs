using Shared.Abstractions.Caching;

namespace Shared.Cache;

public class CacheOptions
{
    public bool UseRedisDistributedCache { get; set; }
    public double ExpirationTimeInMinute { get; set; } = 30;
    public double LocalCacheExpirationTimeInMinute { get; set; } = 5;
    public CacheSerializationType SerializationType { get; set; } = CacheSerializationType.Json;
    public RedisDistributedCacheOptions? RedisCacheOptions { get; set; } = default!;
    public string DefaultCachePrefix { get; set; } = "Ch_";
}

public class RedisDistributedCacheOptions
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public bool AllowAdmin { get; set; }
}
