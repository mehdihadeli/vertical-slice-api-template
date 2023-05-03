using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Abstractions.Web;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;

namespace Vertical.Slice.Template.Products;

internal class ProductConfigurations : IModuleConfiguration
{
    public const string Tag = "Products";
    public const string ProductsPrefixUri = "api/v{version:apiVersion}/catalogs/products";

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
        var productsV1 = products.MapGroup(ProductsPrefixUri).HasDeprecatedApiVersion(0.9).HasApiVersion(1.0);

        productsV1.MapCreateProductEndpoint();
        productsV1.MapGetProductByIdEndpoint();
        productsV1.MapGetProductsByPageEndpoint();

        return endpoints;
    }
}
