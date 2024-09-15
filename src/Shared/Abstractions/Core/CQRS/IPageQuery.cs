using Shared.Abstractions.Core.Paging;

namespace Shared.Abstractions.Core.CQRS;

public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : class;
