using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Shared.Core.Contracts;
using Shared.Core.Types;
using Shared.Web.Contracts;
using Shared.Web.Extensions;
using Shared.Web.Minimal.Extensions;
using Shared.Web.ProblemDetail.HttpResults;
using Vertical.Slice.Template.Products.Dtos;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;

namespace Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;

internal static class GetProductsEndpoint
{
    internal static RouteHandlerBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        return
        // app.MapQueryEndpoint<GetProductsByPageRequestParameters, GetGetProductsByPageResponse, GetProductsByPage,
        //         GetProductsByPageResult>("/")
        app.MapGet("/", Handle)
            .WithName(nameof(GetProductsByPage))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Getting products by page info", "Getting products by page info")
            // .Produces<GetGetProductsByPageResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            .WithDisplayName("Get products by page info.")
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetGetProductsByPageResponse>, ValidationProblem>> Handle(
            [AsParameters] GetProductsByPageRequestParameters requestParameters
        )
        {
            var (context, mediatr, mapper, cancellationToken, _, _, _, _) = requestParameters;

            var query = mapper.Map<GetProductsByPage>(requestParameters);

            var result = await mediatr.Send(query, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.Ok(new GetGetProductsByPageResponse(result.Products));
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

internal record GetGetProductsByPageResponse(IPageList<ProductDto> Products);
