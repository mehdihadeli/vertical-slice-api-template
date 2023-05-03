using MediatR;

namespace Shared.Abstractions.Core.Paging;

public interface IPageQuery<out TResponse> : IPageRequest, IRequest<TResponse>
    where TResponse : class { }
