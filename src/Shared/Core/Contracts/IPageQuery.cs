using MediatR;
using Shared.Core.Types;

namespace Shared.Core.Contracts;

public interface IPageQuery<out TResponse> : IPageRequest, IRequest<TResponse>
    where TResponse : class { }
