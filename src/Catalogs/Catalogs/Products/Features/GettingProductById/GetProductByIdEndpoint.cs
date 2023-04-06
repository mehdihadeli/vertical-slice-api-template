using Catalogs.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;

namespace Catalogs.Products.Features.GettingProductById;

internal static class GetProductByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", Handle)
            .WithName(nameof(GetProductById))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Getting a product by id", "Getting a product by id")
            .Produces<GetProductByIdResponse>("Product fetched successfully.", StatusCodes.Status200OK)
            .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            .ProducesProblem("Product not found", StatusCodes.Status404NotFound)
            .WithDisplayName("Get a product by Id.")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> Handle(
        Guid id,
        HttpContext context,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetProductById(id), cancellationToken);

        return Results.Ok(new GetProductByIdResponse(result.Product));
    }
}

internal record GetProductByIdResponse(ProductLiteDto Product);
