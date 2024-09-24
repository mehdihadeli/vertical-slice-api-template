using EasyCaching.Core;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Caching;

namespace Shared.Cache.Behaviours;

public class InvalidateCachingBehavior<TRequest, TResponse>(
    ILogger<InvalidateCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory.GetCachingProvider(
        cacheOptions.Value.DefaultCacheType
    );

    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message is not IInvalidateCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // No cache policy found, so just continue through the pipeline
            return await next(message, cancellationToken);
        }

        var cacheKeys = cacheRequest.CacheKeys(message);
        var response = await next(message, cancellationToken);

        foreach (var cacheKey in cacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
            logger.LogDebug("Cache data with cache key: {CacheKey} invalidated", cacheKey);
        }

        return response;
    }
}

public class StreamInvalidateCachingBehavior<TRequest, TResponse>(
    ILogger<StreamInvalidateCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory.GetCachingProvider(
        cacheOptions.Value.DefaultCacheType
    );

    public async IAsyncEnumerable<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        StreamHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message is not IStreamInvalidateCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // If the request does not implement IStreamCacheRequest, go to the next pipeline
            await foreach (var response in next(message, cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        await foreach (var response in next(message, cancellationToken))
        {
            var cacheKeys = cacheRequest.CacheKeys(message);

            foreach (var cacheKey in cacheKeys)
            {
                await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
                logger.LogDebug("Cache data with cache key: {CacheKey} invalidated", cacheKey);
            }

            yield return response;
        }
    }
}
