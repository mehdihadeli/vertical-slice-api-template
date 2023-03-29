using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalogs.Products.Features.CreatingProduct;

internal static class CreateProductEndpoint
{
	public static RouteHandlerBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
	{
		return app.MapPost("api/products", Handle)
			.WithName(nameof(CreateProduct))
			.WithTags(nameof(Product))
			.Produces(StatusCodes.Status404NotFound)
			.Produces(StatusCodes.Status201Created);
	}

	private static async Task<IResult> Handle(
		CreateProductRequest request,
		HttpContext context,
		IMediator mediator,
		IMapper mapper,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(request, nameof(request));

		var command = mapper.Map<CreateProduct>(request);

		var result = await mediator.Send(command, cancellationToken);

		return Results.CreatedAtRoute("GetProductById", new {id = result.Id}, result);
	}
}

internal record CreateProductRequest(string Name, Guid CategoryId, decimal Price, string? Description);