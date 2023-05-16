using MediatR;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;

public interface ICommand : IRequest { }

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : class { }
