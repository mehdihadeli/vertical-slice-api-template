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

    private static async Task<IResult> Handle([AsParameters] GetGetProductsByPageRequest request)
    {
        var mediatr = request.Mediator;
        var cancellationToken = request.CancellationToken;
        var result = await mediatr.Send(
            new GetProductByPage(request.PageSize, request.PageNumber, request.Filters, request.SortOrder),
            cancellationToken
        );

        return Results.Ok(new GetGetProductsByPageResponse(result.Products));
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetGetProductsByPageRequest(
    HttpContext Context,
    IMediator Mediator,
    CancellationToken CancellationToken,
    int PageSize = 10,
    int PageNumber = 1,
    string? Filters = null,
    string? SortOrder = null
) : PageRequest(PageSize, PageNumber, Filters, SortOrder);

internal record GetGetProductsByPageResponse(IPageList<ProductDto> Products);
