using Catalogs.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalogs.Products.Features.GettingProductById;

internal static class GetProductByIdEndpoint
{
	public static RouteHandlerBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
	{
		return app.MapGet("/{id:guid}", Handle)
			.WithName(nameof(GetProductById))
			.WithTags(Configuratinos.Tag)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status200OK);
	}

	private static async Task<IResult> Handle(
		Guid id,
		HttpContext context,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var result = await mediator.Send(new GetProductById(id), cancellationToken);

		return Results.Ok(new GetProductByIdResponse(result.Product));
	}
}

internal record GetProductByIdResponse(ProductLiteDto Product);