using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Caching;
using Shared.Cache.Serializers.MessagePack;
using Shared.Core.Extensions;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using StackExchange.Redis;

namespace Shared.Cache;

#pragma warning disable EXTEXP0018

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddCustomCaching(this WebApplicationBuilder builder)
    {
        builder.Services.AddConfigurationOptions<CacheOptions>(nameof(CacheOptions));
        var cacheOptions = builder.Configuration.BindOptions<CacheOptions>(nameof(CacheOptions));

        builder.Services.AddSingleton<ICacheService, CacheService>();

        // https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid
        // https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview
        // If the app has an IDistributedCache implementation, the HybridCache service uses it for secondary caching. This two-level caching strategy allows HybridCache to provide the speed of an in-memory cache and the durability of a distributed or persistent cache.
        if (cacheOptions.UseRedisDistributedCache)
        {
            builder.Services.AddSingleton<IRedisPubSubService, RedisPubSubService>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                ArgumentNullException.ThrowIfNull(cacheOptions.RedisCacheOptions);
                return ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = { $"{cacheOptions.RedisCacheOptions.Host}:{cacheOptions.RedisCacheOptions.Port}" },
                        AllowAdmin = cacheOptions.RedisCacheOptions.AllowAdmin,
                    }
                );
            });
        }

        var hybridCacheBuilder = builder.Services.AddHybridCache();

        switch (cacheOptions.SerializationType)
        {
            case CacheSerializationType.MessagePack:
                {
                    hybridCacheBuilder.AddSerializerFactory<MessagePackHybridCacheSerializerFactory>();
                }

                break;
        }

        return builder;
    }
}