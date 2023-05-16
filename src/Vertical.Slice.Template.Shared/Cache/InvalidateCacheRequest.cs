using MediatR;
using Vertical.Slice.Template.Shared.Abstractions.Caching;

namespace Vertical.Slice.Template.Shared.Cache;

public abstract class InvalidateCacheRequest<TRequest, TResponse> : IInvalidateCacheRequest<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public virtual string Prefix => "Ch_";
    public abstract IEnumerable<string> CacheKeys(TRequest request);
}

public abstract class InvalidateCacheRequest<TRequest> : IInvalidateCacheRequest<TRequest>
    where TRequest : IRequest<Unit>
{
    public virtual string Prefix => "Ch_";
    public abstract IEnumerable<string> CacheKeys(TRequest request);
}
