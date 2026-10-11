using Vertical.Slice.Template.ServiceDefaults;
using Shared.OpenApi.AspnetOpenApi.Extensions;
using Shared.Web.Extensions;
using Shared.Web.Extensions.WebApplicationBuilderExtensions;
using Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Shared;
using Vertical.Slice.Template.Shared.Clients;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationExtensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Host.UseDefaultServiceProvider(
        (context, options) =>
        {
            var isDevMode =
                context.HostingEnvironment.IsDevelopment()
                || context.HostingEnvironment.IsTest()
                || context.HostingEnvironment.IsStaging();

            // Handling Captive Dependency Problem
            // https://ankitvijay.net/2020/03/17/net-core-and-di-beware-of-captive-dependency/
            // https://levelup.gitconnected.com/top-misconceptions-about-dependency-injection-in-asp-net-core-c6a7afd14eb4
            // https://blog.ploeh.dk/2014/06/02/captive-dependency/
            // https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-2.2#scope-validation
            // CreateDefaultBuilder and WebApplicationBuilder in minimal apis sets `ServiceProviderOptions.ValidateScopes` and `ServiceProviderOptions.ValidateOnBuild` to true if the app's environment is Development.
            // check dependencies are used in a valid lifetime scope
            options.ValidateScopes = isDevMode;
            // validate dependencies on the startup immediately instead of waiting for using the service
            options.ValidateOnBuild = isDevMode;
        }
    );

    // #if EnableSwagger
    builder.AddAspnetOpenApi(["v1"]);
    // #endif

    builder.AddInfrastructures();

    builder.AddCatalogsServices();

    builder.AddCustomVersioning();

    var app = builder.Build();

    if (app.Environment.IsDependencyTest())
    {
        return;
    }

    app.UseDefaultServices();
    await app.UseInfrastructure();
    await app.UseCatalogs();
    app.MapApplicationEndpoints();
    app.MapDefaultEndpoints();

    // #if EnableSwagger
    if (app.Environment.IsDevelopment())
    {
        app.UseAspnetOpenApi();
    }
    // #endif

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application terminated unexpectedly: {ex}");
}
