using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Vertical.Slice.Template.Shared.Abstractions.Caching;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Web.Extensions;

namespace Vertical.Slice.Template.Shared.Cache;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomCaching(this WebApplicationBuilder builder)
    {
        // https://www.twilio.com/blog/provide-default-configuration-to-dotnet-applications
        var cacheOptions = builder.Configuration.BindOptions<CacheOptions>();
        cacheOptions.NotBeNull();

        builder.Services.AddEasyCaching(option =>
        {
            if (cacheOptions.RedisCacheOptions is not null)
            {
                option.UseRedis(
                    config =>
                    {
                        config.DBConfig = new RedisDBOptions
                        {
                            Configuration = cacheOptions.RedisCacheOptions.ConnectionString
                        };
                        config.SerializerName = cacheOptions.SerializationType;
                    },
                    nameof(CacheProviderType.Redis)
                );
            }

            option.UseInMemory(
                config =>
                {
                    config.SerializerName = cacheOptions.SerializationType;
                },
                nameof(CacheProviderType.InMemory)
            );

            if (cacheOptions.SerializationType == nameof(CacheSerializationType.Json))
            {
                option.WithJson(
                    jsonSerializerSettingsConfigure: x =>
                    {
                        x.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                    },
                    nameof(CacheSerializationType.Json)
                );
            }
            else if (cacheOptions.SerializationType == nameof(CacheSerializationType.MessagePack))
            {
                option.WithMessagePack(nameof(CacheSerializationType.MessagePack));
            }
        });

        return builder;
    }
}
