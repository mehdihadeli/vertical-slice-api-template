using Mediator;

namespace Shared.Abstractions.Core.CQRS;

public interface ICommand : IRequest;

public interface ICommand<TResponse> : IRequest<TResponse>
    where TResponse : class;
