using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.GettingProductById;
using Catalogs.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Web;

internal class Configuratinos : IModuleConfiguration
{
	public const string Tag = "Products";
	public const string ProductsPrefixUri = $"{Configurations.CatalogModulePrefixUri}/products";
	public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
	{
		return builder;
	}

	public Task<WebApplication> ConfigureModule(WebApplication app)
	{
		return Task.FromResult(app);
	}

	public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
	{
		var products = endpoints.NewVersionedApi(Tag);
		var productsV1 = products.MapGroup(ProductsPrefixUri)
			.HasDeprecatedApiVersion(0.9)
			.HasApiVersion(1.0);

		productsV1.MapCreateProductEndpoint();
		productsV1.MapGetProductByIdEndpoint();

		return endpoints;
	}
}