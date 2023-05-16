using MediatR;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : class { }

// https://jimmybogard.com/mediatr-10-0-released/
public interface IStreamQueryHandler<in TQuery, TResponse> : IStreamRequestHandler<TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
    where TResponse : class { }
