using AutoMapper;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Products.Dtos;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Shared.Abstractions.Web;
using Vertical.Slice.Template.Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Shared.Web.ProblemDetail.HttpResults;

namespace Vertical.Slice.Template.Products.Features.GettingProductById.v1;

internal static class GetProductByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
    {
        // return app.MapQueryEndpoint<GetProductByIdRequestParameters, GetProductByIdResponse, GetProductById,
        //         GetProductByIdResult>("/{id:guid}")
        return app.MapGet("/{id:guid}", Handle)
            .WithName(nameof(GetProductById))
            .WithDisplayName(nameof(GetProductById).Humanize())
            .WithSummaryAndDescription(nameof(GetProductById).Humanize(), nameof(GetProductById).Humanize())
            .WithTags(ProductConfigurations.Tag)
            // .Produces<GetProductByIdResponse>("Product fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem("Invalid input for getting product.", StatusCodes.Status400BadRequest)
            // .ProducesProblem("Product not found", StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetProductByIdResponse>, ValidationProblem, NotFoundHttpProblemResult>> Handle(
            [AsParameters] GetProductByIdRequestParameters requestParameters
        )
        {
            var (id, _, mediator, mapper, cancellationToken) = requestParameters;
            var result = await mediator.Send(new GetProductById(id), cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.Ok(new GetProductByIdResponse(result.Product));
        }
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#parameter-binding-for-argument-lists-with-asparameters
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding#binding-precedence
internal record GetProductByIdRequestParameters(
    [FromRoute] Guid Id,
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

internal record GetProductByIdResponse(ProductDto Product);
