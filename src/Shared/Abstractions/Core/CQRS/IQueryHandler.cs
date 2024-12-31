using Mediator;

namespace Shared.Abstractions.Core.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull;

public interface IStreamQueryHandler<in TQuery, out TResponse> : IStreamRequestHandler<TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
    where TResponse : notnull;
