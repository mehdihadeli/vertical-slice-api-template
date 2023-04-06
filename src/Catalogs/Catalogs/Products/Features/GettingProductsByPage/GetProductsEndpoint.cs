using Catalogs.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Core.Wrappers;
using Shared.Web.Extensions;

namespace Catalogs.Products.Features.GettingProductsByPage;

internal static class GetProductsEndpoint
{
    internal static RouteHandlerBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", Handle)
            .WithName(nameof(GetProductByPage))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Getting products by page info", "Getting products by page info")
            .Produces<GetGetProductsByPageResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            .WithDisplayName("Get products by page info.")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> Handle([AsParameters] GetProductByPageParameters parameters)
    {
        var (request, _, mediator, cancellationToken) = parameters;
        var result = await mediator.Send(
            new GetProductByPage
            {
                Filters = request.Filters,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortOrder = request.SortOrder
            },
            cancellationToken
        );

        return Results.Ok(new GetGetProductsByPageResponse(result.Products));
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetProductByPageParameters(
    GetGetProductsByPageRequest Request,
    HttpContext Context,
    IMediator Mediator,
    CancellationToken CancellationToken
);

internal record GetGetProductsByPageRequest : PageRequest;

internal record GetGetProductsByPageResponse(IPageList<ProductDto> Products);
