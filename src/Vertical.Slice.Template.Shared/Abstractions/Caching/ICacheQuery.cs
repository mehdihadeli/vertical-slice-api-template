using Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;

namespace Vertical.Slice.Template.Shared.Abstractions.Caching;

public interface ICacheQuery<in TRequest, TResponse> : IQuery<TResponse>, ICacheRequest<TRequest, TResponse>
    where TResponse : class
    where TRequest : IQuery<TResponse> { }
