using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

namespace Vertical.Slice.Template.Shared;

public static class CatalogsConfigurations
{
    public const string CatalogsPrefixUri = "api/v{version:apiVersion}";

    public static WebApplicationBuilder AddCatalogsServices(this WebApplicationBuilder builder)
    {
        // Catalogs Configurations
        builder.AddStorage();

        // Modules
        builder.AddProductsModuleServices();

        return builder;
    }

    public static async Task<WebApplication> UseCatalogs(this WebApplication app)
    {
        // Modules
        await app.UseProductsModule();

        return app;
    }

    public static IEndpointRouteBuilder MapCatalogsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Shared
        endpoints.MapGet("/", () => "Catalogs  Api.").ExcludeFromDescription();

        // Modules
        endpoints.MapProductsModuleEndpoints();

        return endpoints;
    }

    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCatalogsEndpoints();
        endpoints.MapModulesEndpoints();

        return endpoints;
    }
}
