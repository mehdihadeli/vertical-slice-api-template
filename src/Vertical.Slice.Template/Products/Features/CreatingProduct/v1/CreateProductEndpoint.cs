using AutoMapper;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Shared.Abstractions.Web;
using Vertical.Slice.Template.Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Shared.Web.ProblemDetail.HttpResults;

namespace Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

internal static class CreateProductEndpoint
{
    internal static RouteHandlerBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        // return app.MapCommandEndpoint<CreateProductRequest, CreateProductResponse, CreateProduct, CreateProductResult>(
        //     "/",
        //     StatusCodes.Status201Created,
        //     getId: response => response.Id
        // );
        return app.MapPost("/", Handle)
            .WithName(nameof(CreateProduct))
            .WithDisplayName(nameof(CreateProduct).Humanize())
            .WithSummaryAndDescription(nameof(CreateProduct).Humanize(), nameof(CreateProduct).Humanize())
            .WithTags(ProductConfigurations.Tag)
            // .Produces<CreateProductResponse>("Product created successfully.", StatusCodes.Status201Created)
            // .ProducesValidationProblem("Invalid input for creating product.", StatusCodes.Status400BadRequest)
            // .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<
            Results<CreatedAtRoute<CreateProductResponse>, UnAuthorizedHttpProblemResult, ValidationProblem>
        > Handle([AsParameters] CreateProductRequestParameters requestParameters)
        {
            var (request, context, mediator, mapper, cancellationToken) = requestParameters;

            var command = mapper.Map<CreateProduct>(request);

            var result = await mediator.Send(command, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.CreatedAtRoute(
                new CreateProductResponse(result.Id),
                "GetProductById",
                new { id = result.Id }
            );
        }
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record CreateProductRequestParameters(
    [FromBody] CreateProductRequest? Request,
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<CreateProductRequest?>;

internal record CreateProductResponse(Guid Id);

// we can expect any value from the user for all reference types are nullable and we should do some validation in other levels (we use pure records mostly for dtos without needing validation)
public record CreateProductRequest(string? Name, Guid CategoryId, decimal Price, string? Description);
