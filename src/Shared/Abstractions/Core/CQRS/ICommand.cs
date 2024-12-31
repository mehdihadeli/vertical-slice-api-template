using Mediator;

namespace Shared.Abstractions.Core.CQRS;

public interface IBaseCommand;

public interface ICommand : ICommand<Unit>;

public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand
    where TResponse : notnull;
