using AutoMapper;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Shared.Abstractions.Core.Paging;
using Vertical.Slice.Template.Shared.Abstractions.Web;
using Vertical.Slice.Template.Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Users.Dtos;

namespace Vertical.Slice.Template.Users.GetUsers;

internal static class GetUsersByPageEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersByPageEndpoint(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", Handle)
            .WithName(nameof(GetUsersByPageEndpoint))
            .WithDisplayName(nameof(GetUsersByPageEndpoint).Humanize())
            .WithSummaryAndDescription(
                nameof(GetUsersByPageEndpoint).Humanize(),
                nameof(GetUsersByPageEndpoint).Humanize()
            )
            .WithTags(UsersConfigurations.Tag)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetUsersByPageResponse>, ValidationProblem>> Handle(
            [AsParameters] GetUsersByPageRequestParameters requestParameters
        )
        {
            var (context, mediatr, mapper, cancellationToken, _, _, _, _) = requestParameters;

            var query = mapper.Map<GetUsersByPage>(requestParameters);

            var result = await mediatr.Send(query, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.Ok(new GetUsersByPageResponse(result.Users));
        }
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetUsersByPageRequestParameters(
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken,
    int PageSize = 10,
    int PageNumber = 1,
    string? Filters = null,
    string? SortOrder = null
) : IHttpQuery, IPageRequest;

internal record GetUsersByPageResponse(IPageList<UserDto> Users);
