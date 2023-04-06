using MediatR;

namespace Shared.Core.Wrappers;

public record PageQuery<TResponse> : PageRequest, IRequest<TResponse>
    where TResponse : class;
