using MediatR;

namespace Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : class { }

// https://jimmybogard.com/mediatr-10-0-released/
public interface IStreamQuery<out T> : IStreamRequest<T>
    where T : notnull { }
