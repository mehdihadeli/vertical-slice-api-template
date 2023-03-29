using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.GettingProductById;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

internal static class Configuratinos
{
	public static WebApplicationBuilder AddProductServices(this WebApplicationBuilder builder)
	{
		return builder;
	}

	public static Task<WebApplication> ConfigureProduct(this WebApplication app)
	{
		return Task.FromResult(app);
	}

	public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapCreateProductEndpoint();
		endpoints.MapGetProductByIdEndpoint();

		return endpoints;
	}
}