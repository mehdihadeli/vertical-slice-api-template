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

    private static async Task<IResult> Handle(
        [AsParameters] GetGetProductsByPageRequest pageRequest,
        HttpContext context,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetProductByPage
            {
                Filters = pageRequest.Filters,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageRequest.PageSize,
                SortOrder = pageRequest.SortOrder
            },
            cancellationToken
        );

        return Results.Ok(new GetGetProductsByPageResponse(result.Products));
    }
}

internal record GetGetProductsByPageRequest : PageRequest;

internal record GetGetProductsByPageResponse(IPageList<ProductDto> Products);
