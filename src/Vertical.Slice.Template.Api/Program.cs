using System.Reflection;
using Serilog;
using Serilog.Events;
using Vertical.Slice.Template;
using Vertical.Slice.Template.Shared;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.Swagger;
using Vertical.Slice.Template.Shared.Web.Minimal.Extensions;

// https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
Log.Logger = new LoggerConfiguration().MinimumLevel
    .Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(
        (context, options) =>
        {
            var isDevMode =
                context.HostingEnvironment.IsDevelopment()
                || context.HostingEnvironment.IsEnvironment("test")
                || context.HostingEnvironment.IsStaging();

            // Handling Captive Dependency Problem
            // https://ankitvijay.net/2020/03/17/net-core-and-di-beware-of-captive-dependency/
            // https://levelup.gitconnected.com/top-misconceptions-about-dependency-injection-in-asp-net-core-c6a7afd14eb4
            // https://blog.ploeh.dk/2014/06/02/captive-dependency/
            // https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-2.2#scope-validation
            // CreateDefaultBuilder and WebApplicationBuilder in minimal apis sets `ServiceProviderOptions.ValidateScopes` and `ServiceProviderOptions.ValidateOnBuild` to true if the app's environment is Development.
            // check dependencies are used in a valid life time scope
            options.ValidateScopes = isDevMode;
            // validate dependencies on the startup immediately instead of waiting for using the service
            options.ValidateOnBuild = isDevMode;
        }
    );

    builder.AddCatalogsServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment() && app.Environment.IsEnvironment("test"))
    {
        app.Services.ValidateDependencies(
            builder.Services,
            typeof(CatalogsMetadata).Assembly,
            Assembly.GetExecutingAssembly()
        );
    }

    await app.UseCatalogs();

    app.MapCatalogsEndpoints();

    app.MapModulesEndpoints();

    // #if EnableSwagger
    if (app.Environment.IsDevelopment())
    {
        // should register as last middleware for discovering all endpoints and its versions correctly
        app.UseCustomSwagger();
    }
    // #endif

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
