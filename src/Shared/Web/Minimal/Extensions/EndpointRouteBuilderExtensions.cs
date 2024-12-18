using Humanizer;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Shared.Abstractions.Web;

namespace Shared.Web.Minimal.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapCommandEndpoint<TRequest, TCommand>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequest, TCommand> mapRequestToCommand
    )
        where TRequest : class
        where TCommand : IRequest
    {
        return builder
            .MapPost(pattern, Handle)
            .WithName(typeof(TCommand).Name)
            .WithDisplayName(typeof(TCommand).Name.Humanize())
            .WithSummary(typeof(TCommand).Name.Humanize())
            .WithDescription(typeof(TCommand).Name.Humanize());

        // we can't generalize all possible type results for auto generating open-api metadata, because it might show unwanted response type as metadata
        async Task<NoContent> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, context, mediator, cancellationToken) = requestParameters;

            var command = mapRequestToCommand(request);
            await mediator.Send(command, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.NoContent();
        }
    }

    public static RouteHandlerBuilder MapCommandEndpoint<TRequest, TResponse, TCommand, TCommandResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        int statusCode,
        Func<TRequest, TCommand> mapRequestToCommand,
        Func<TCommandResult, TResponse> mapCommandResultToResponse,
        Func<TResponse, Guid>? getId = null
    )
        where TRequest : class
        where TResponse : class
        where TCommandResult : class
        where TCommand : IRequest<TCommandResult>
    {
        return builder
            .MapPost(pattern, Handle)
            .WithName(typeof(TCommand).Name)
            .WithDisplayName(typeof(TCommand).Name.Humanize())
            .WithSummary(typeof(TCommand).Name.Humanize())
            .WithDescription(typeof(TCommand).Name.Humanize());

        // https://github.com/dotnet/aspnetcore/issues/47630
        // we can't generalize all possible type results for auto generating open-api metadata, because it might show unwanted response type as metadata
        async Task<IResult> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, context, mediator, cancellationToken) = requestParameters;
            var host = $"{context.Request.Scheme}://{context.Request.Host}";

            var command = mapRequestToCommand(request);
            var res = await mediator.Send(command, cancellationToken);

            var response = mapCommandResultToResponse(res);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return statusCode switch
            {
                StatusCodes.Status201Created => getId is { }
                    ? TypedResults.Created($"{host}{pattern}/{getId(response)}", response)
                    : TypedResults.Ok(response),
                StatusCodes.Status401Unauthorized => TypedResultsExtensions.UnAuthorizedProblem(),
                StatusCodes.Status500InternalServerError => TypedResultsExtensions.InternalProblem(),
                StatusCodes.Status202Accepted => TypedResults.Accepted(new Uri($"{host}{pattern}"), response),
                _ => TypedResults.Ok(response),
            };
        }
    }

    public static RouteHandlerBuilder MapQueryEndpoint<TRequestParameters, TResponse, TQuery, TQueryResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequestParameters, TQuery> mapRequestToQuery,
        Func<TQueryResult, TResponse> mapQueryResultToResponse
    )
        where TRequestParameters : IHttpQuery
        where TResponse : class
        where TQueryResult : class
        where TQuery : IRequest<TQueryResult>
    {
        return builder
            .MapGet(pattern, Handle)
            .WithName(typeof(TQuery).Name)
            .WithDisplayName(typeof(TQuery).Name.Humanize())
            .WithSummary(typeof(TQuery).Name.Humanize())
            .WithDescription(typeof(TQuery).Name.Humanize());

        // we can't generalize all possible type results for auto generating open-api metadata, because it might show unwanted response type as metadata
        async Task<Ok<TResponse>> Handle([AsParameters] TRequestParameters requestParameters)
        {
            var mediator = requestParameters.Mediator;
            var cancellationToken = requestParameters.CancellationToken;

            var query = mapRequestToQuery(requestParameters);

            var res = await mediator.Send(query, cancellationToken);

            var response = mapQueryResultToResponse(res);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.Ok(response);
        }
    }
}
