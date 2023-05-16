using Vertical.Slice.Template.Shared.Abstractions.Core.Paging;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;

public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : class { }
