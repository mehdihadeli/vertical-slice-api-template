using Mediator;

namespace Shared.Abstractions.Core.CQRS;

public interface IBaseQuery;

public interface IBaseStreamQuery;

public interface IQuery<out TResponse> : IBaseQuery, IRequest<TResponse>
    where TResponse : notnull;

public interface IStreamQuery<out T> : IBaseStreamQuery, IStreamRequest<T>
    where T : notnull;
