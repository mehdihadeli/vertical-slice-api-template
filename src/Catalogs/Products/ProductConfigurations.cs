using AutoMapper;
using Catalogs.Products.Data.Executors;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.GettingProductById;
using Catalogs.Products.Features.GettingProductsByPage;
using Catalogs.Products.ReadModel;
using Catalogs.Shared;
using Catalogs.Shared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Wrappers;
using Shared.EF.Extensions;
using Shared.Web;

internal class ProductConfigurations : IModuleConfiguration
{
	public const string Tag = "Products";
	public const string ProductsPrefixUri = $"{CatalogConfigurations.CatalogModulePrefixUri}/products";

	public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
	{
		// Db related operations for injection as dependencies
		builder.Services.AddTransient<CreateAndSaveProductExecutor>(
			sp =>
			{
				var context = sp.GetRequiredService<CatalogsDbContext>();
				return (entity, cancellationToken) => context.InsertAndSaveAsync(entity, cancellationToken);
			});

		builder.Services.AddTransient<GetProductByIdExecutor>(
			sp =>
			{
				var context = sp.GetRequiredService<CatalogsDbContext>();
				var mapper = sp.GetRequiredService<IMapper>();

				Task<ProductReadModel?> Query(Guid id, CancellationToken cancellationToken) =>
					context.ProjectEntityAsync<Product, ProductReadModel>(
							mapper.ConfigurationProvider,
							cancellationToken)
						.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

				return Query;
			});

		builder.Services.AddTransient<GetProductsExecutor>(
			sp =>
			{
				var context = sp.GetRequiredService<CatalogsDbContext>();
				var mapper = sp.GetRequiredService<IMapper>();

				IQueryable<ProductReadModel> Query(IPageRequest pageRequest, CancellationToken cancellationToken)
				{
					var collection = context.ProjectEntityAsync<Product, ProductReadModel>(
						mapper.ConfigurationProvider,
						cancellationToken);

					return collection;
				}

				return Query;
			});

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
		productsV1.MapGetProductsEndpoint();

		return endpoints;
	}
}