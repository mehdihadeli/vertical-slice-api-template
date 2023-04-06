using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Extensions;

namespace Catalogs.Products.Features.CreatingProduct;

internal static class CreateProductEndpoint
{
    internal static RouteHandlerBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", Handle)
            .WithName(nameof(CreateProduct))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Creating a New Product", "Creating a New Product")
            .Produces<CreateProductResponse>("Product created successfully.", StatusCodes.Status201Created)
            .ProducesValidationProblem("Invalid input for creating product.", StatusCodes.Status400BadRequest)
            .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
            .WithDisplayName("Create a new product.")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> Handle(
        CreateProductRequest request,
        HttpContext context,
        IMediator mediator,
        IMapper mapper,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request, nameof(request));

        var command = mapper.Map<CreateProduct>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.CreatedAtRoute<CreateProductResponse>(
            "GetProductById",
            new { id = result.Id },
            new CreateProductResponse(result.Id)
        );
    }
}

internal record CreateProductRequest(string Name, Guid CategoryId, decimal Price, string? Description);

internal record CreateProductResponse(Guid Id);
