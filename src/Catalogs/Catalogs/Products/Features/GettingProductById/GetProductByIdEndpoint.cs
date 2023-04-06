using Catalogs.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    private static async Task<IResult> Handle([AsParameters] GetProductByIdParameters parameters)
    {
        var (id, _, mediator, cancellationToken) = parameters;
        var result = await mediator.Send(new GetProductById(id), cancellationToken);

        return Results.Ok(new GetProductByIdResponse(result.Product));
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetProductByIdParameters(
    [FromRoute] Guid Id,
    HttpContext Context,
    IMediator Mediator,
    CancellationToken CancellationToken
);

internal record GetProductByIdResponse(ProductLiteDto Product);
