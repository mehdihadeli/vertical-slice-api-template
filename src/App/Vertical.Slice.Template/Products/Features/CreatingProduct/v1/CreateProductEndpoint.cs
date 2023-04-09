using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Validation.Extensions;
using Shared.Web.Contracts;
using Shared.Web.Extensions;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

internal static class CreateProductEndpoint
{
    internal static RouteHandlerBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        return
        //app.MapPost("/", Handle)
        app.MapCommandEndpoint<CreateProductRequest, CreateProductResponse, CreateProduct, CreateProductResult>(
                "/",
                StatusCodes.Status201Created
            )
            .WithName(nameof(CreateProduct))
            .WithTags(ProductConfigurations.Tag)
            .WithSummaryAndDescription("Creating a New Product", "Creating a New Product")
            .Produces<CreateProductResponse>("Product created successfully.", StatusCodes.Status201Created)
            .ProducesValidationProblem("Invalid input for creating product.", StatusCodes.Status400BadRequest)
            .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
            .WithDisplayName("Create a new product.")
            .MapToApiVersion(1.0);

        async Task<IResult> Handle([AsParameters] CreateProductRequestParameters requestInput)
        {
            var (request, context, mediator, mapper, cancellationToken) = requestInput;

            request.NotNull();

            var command = mapper.Map<CreateProduct>(request);

            var result = await mediator.Send(command, cancellationToken);

            return Results.CreatedAtRoute(
                "GetProductById",
                new { id = result.Id },
                new CreateProductResponse(result.Id)
            );
        }
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record CreateProductRequestParameters(
    [FromBody] CreateProductRequest Request,
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<CreateProductRequest>;

internal record CreateProductResponse(Guid Id);

// we can expect any value from the user for all reference types are nullable and we should do some validation in other levels (we use pure records mostly for dtos without needing validation)
internal record CreateProductRequest(string Name, Guid CategoryId, decimal Price, string? Description);
