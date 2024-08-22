using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationExtensions;
using Vertical.Slice.Template.Users;

namespace Vertical.Slice.Template.Shared;

public static class CatalogsConfigurations
{
    public const string CatalogsPrefixUri = "api/v{version:apiVersion}";

    public static WebApplicationBuilder AddCatalogsServices(this WebApplicationBuilder builder)
    {
        // Shared
        // Infrastructure
        builder.AddInfrastructures();

        // Shared
        // Catalogs Configurations
        builder.AddStorage();

        // Modules
        builder.AddProductsModuleServices();
        builder.AddUsersModuleServices();

        return builder;
    }

    public static async Task<WebApplication> UseCatalogs(this WebApplication app)
    {
        // Shared
        await app.UseInfrastructure();

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
        endpoints.MapUsersEndpoints();

        return endpoints;
    }
}
