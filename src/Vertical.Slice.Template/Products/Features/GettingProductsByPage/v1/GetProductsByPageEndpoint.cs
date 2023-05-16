using AutoMapper;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Shared.Abstractions.Core.Paging;
using Vertical.Slice.Template.Shared.Abstractions.Web;
using Vertical.Slice.Template.Shared.Web.Minimal.Extensions;

namespace Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;

internal static class GetProductsByPageEndpoint
{
    internal static RouteHandlerBuilder MapGetProductsByPageEndpoint(this IEndpointRouteBuilder app)
    {
        // return app.MapQueryEndpoint<GetProductsByPageRequestParameters, GetProductsByPageResponse, GetProductsByPage,
        //         GetProductsByPageResult>("/")
        return app.MapGet("/", Handle)
            .WithName(nameof(GetProductsByPage))
            .WithDisplayName(nameof(GetProductsByPage).Humanize())
            .WithSummaryAndDescription(nameof(GetProductsByPage).Humanize(), nameof(GetProductsByPage).Humanize())
            .WithTags(ProductConfigurations.Tag)
            // .Produces<GetProductsByPageResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetProductsByPageResponse>, ValidationProblem>> Handle(
            [AsParameters] GetProductsByPageRequestParameters requestParameters
        )
        {
            var (context, mediatr, mapper, cancellationToken, _, _, _, _) = requestParameters;

            var query = mapper.Map<GetProductsByPage>(requestParameters);

            var result = await mediatr.Send(query, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.Ok(new GetProductsByPageResponse(result.Products));
        }
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetProductsByPageRequestParameters(
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken,
    int PageSize = 10,
    int PageNumber = 1,
    string? Filters = null,
    string? SortOrder = null
) : IHttpQuery, IPageRequest;

internal record GetProductsByPageResponse(IPageList<ProductDto> Products);
