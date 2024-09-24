using Mediator;

namespace Shared.Abstractions.Core.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : class;

public interface IStreamQuery<out T> : IStreamRequest<T>
    where T : notnull;
