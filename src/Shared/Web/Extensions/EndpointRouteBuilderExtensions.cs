using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Contracts;

namespace Shared.Web.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapCommandEndpoint<TRequest, TCommand>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequest, TCommand>? mapRequestToCommand = null
    )
        where TRequest : class
        where TCommand : IRequest
    {
        return builder.MapPost(pattern, Handle);

        async Task<IResult> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, context, mediator, mapper, cancellationToken) = requestParameters;

            var command = mapRequestToCommand is not null
                ? mapRequestToCommand(request)
                : mapper.Map<TCommand>(request);
            await mediator.Send(command, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            return TypedResults.NoContent();
        }
    }

    public static RouteHandlerBuilder MapCommandEndpoint<TRequest, TResponse, TCommand, TCommandResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        int statusCode,
        Func<TRequest, TCommand>? mapRequestToCommand = null,
        Func<TCommandResult, TResponse>? mapCommandResultToResponse = null
    )
        where TRequest : class
        where TResponse : class
        where TCommandResult : class
        where TCommand : IRequest<TCommandResult>
    {
        return builder.MapPost(pattern, Handle);

        async Task<IResult> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, context, mediator, mapper, cancellationToken) = requestParameters;

            var command = mapRequestToCommand is not null
                ? mapRequestToCommand(request)
                : mapper.Map<TCommand>(request);
            var res = await mediator.Send(command, cancellationToken);

            var response = mapCommandResultToResponse is not null
                ? mapCommandResultToResponse(res)
                : mapper.Map<TResponse>(res);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            return TypedResults.Json(response, statusCode: statusCode);
        }
    }

    public static RouteHandlerBuilder MapQueryEndpoint<TRequestParameters, TResponse, TQuery, TQueryResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequestParameters, TQuery>? mapRequestToQuery = null,
        Func<TQueryResult, TResponse>? mapQueryResultToResponse = null
    )
        where TRequestParameters : IHttpQuery
        where TResponse : class
        where TQueryResult : class
        where TQuery : IRequest<TQueryResult>
    {
        return builder.MapGet(pattern, Handle);

        async Task<IResult> Handle([AsParameters] TRequestParameters requestParameters)
        {
            var mediator = requestParameters.Mediator;
            var mapper = requestParameters.Mapper;
            var cancellationToken = requestParameters.CancellationToken;

            var query = mapRequestToQuery is not null
                ? mapRequestToQuery(requestParameters)
                : mapper.Map<TQuery>(requestParameters);

            var res = await mediator.Send(query, cancellationToken);

            var response = mapQueryResultToResponse is not null
                ? mapQueryResultToResponse(res)
                : mapper.Map<TResponse>(res);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            return TypedResults.Ok(response);
        }
    }
}
