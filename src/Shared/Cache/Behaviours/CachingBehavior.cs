using EasyCaching.Core;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Caching;

namespace Shared.Cache.Behaviours;

// Ref: https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
public class CachingBehavior<TRequest, TResponse>(
    ILogger<CachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory.GetCachingProvider(
        cacheOptions.Value.DefaultCacheType
    );

    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message is not ICacheRequest<TRequest, TResponse> cacheRequest)
        {
            // No cache policy found, so just continue through the pipeline
            return await next(message, cancellationToken);
        }

        var cacheKey = cacheRequest.CacheKey(message);
        var cachedResponse = await _cacheProvider.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse.Value != null)
        {
            logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );

            return cachedResponse.Value;
        }

        var response = await next(message, cancellationToken);

        var expiredTimeSpan = GetExpirationTime(cacheRequest);

        await _cacheProvider.SetAsync(cacheKey, response, expiredTimeSpan, cancellationToken);

        logger.LogDebug(
            "Caching response for {TRequest} with cache key: {CacheKey}",
            typeof(TRequest).FullName,
            cacheKey
        );

        return response;
    }

    private TimeSpan GetExpirationTime(ICacheRequest<TRequest, TResponse> cacheRequest)
    {
        if (cacheRequest.AbsoluteExpirationRelativeToNow != TimeSpan.FromMinutes(5))
        {
            return cacheRequest.AbsoluteExpirationRelativeToNow;
        }
        else if (TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute) != TimeSpan.FromMinutes(5))
        {
            return TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute);
        }

        return cacheRequest.AbsoluteExpirationRelativeToNow;
    }
}

public class StreamCachingBehavior<TRequest, TResponse>(
    ILogger<StreamCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory.GetCachingProvider(
        cacheOptions.Value.DefaultCacheType
    );
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async IAsyncEnumerable<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        StreamHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message is not IStreamCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // If the request does not implement IStreamCacheRequest, go to the next pipeline
            await foreach (var response in next(message, cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        var cacheKey = cacheRequest.CacheKey(message);
        var cachedResponse = await _cacheProvider.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );

            yield return cachedResponse.Value;
            yield break;
        }

        var expiredTimeSpan = GetExpirationTime(cacheRequest);

        await foreach (var response in next(message, cancellationToken))
        {
            await _cacheProvider.SetAsync(cacheKey, response, expiredTimeSpan, cancellationToken);

            logger.LogDebug(
                "Caching response for {TRequest} with cache key: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );

            yield return response;
        }
    }

    private TimeSpan GetExpirationTime(IStreamCacheRequest<TRequest, TResponse> cacheRequest)
    {
        if (cacheRequest.AbsoluteExpirationRelativeToNow != TimeSpan.FromMinutes(5))
        {
            return cacheRequest.AbsoluteExpirationRelativeToNow;
        }
        else if (TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute) != TimeSpan.FromMinutes(5))
        {
            return TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute);
        }

        return cacheRequest.AbsoluteExpirationRelativeToNow;
    }
}
