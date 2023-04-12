using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Shared.Core.Contracts;
using Shared.Web.Contracts;
using Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Products.Dtos;

namespace Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;

internal static class GetProductsByPageEndpoint
{
    internal static RouteHandlerBuilder MapGetProductsByPageEndpoint(this IEndpointRouteBuilder app)
    {
        return
        // app.MapQueryEndpoint<GetProductsByPageRequestParameters, GetProductsByPageResponse, GetProductsByPage,
        //         GetProductsByPageResult>("/")
        app.MapGet("/", Handle)
            .WithName(nameof(GetProductsByPage))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Getting products by page info", "Getting products by page info")
            // .Produces<GetProductsByPageResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            .WithDisplayName("Get products by page info.")
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
