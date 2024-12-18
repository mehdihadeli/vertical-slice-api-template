using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Shared.Logging.Extensions;
using Shared.Swagger;
using Shared.Web.Extensions;
using Shared.Web.Minimal.Extensions;
using Vertical.Slice.Template.Shared;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

// https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
// https://github.com/serilog/serilog-extensions-hosting
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddCustomSerilog();

    builder.AddAppProblemDetails();

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

    builder.AddCatalogsServices();

    var app = builder.Build();

    if (app.Environment.IsDependencyTest())
    {
        return;
    }

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
    // https://github.com/dotnet/aspnetcore/pull/26567
    app.UseExceptionHandler(options: new ExceptionHandlerOptions { AllowStatusCode404Response = true });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsTest())
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errrors
        app.UseDeveloperExceptionPage();
    }

    // this middleware should be first middleware
    // request logging just log in information level and above as default
    app.UseSerilogRequestLogging();

    await app.UseCatalogs();

    app.MapCatalogsEndpoints();

    app.MapModulesEndpoints();

    // #if EnableSwagger
    if (app.Environment.IsDevelopment())
    {
        // should register as last middleware for discovering all endpoints and its versions correctly
        app.UseCustomSwagger();

        // https://github.com/scalar/scalar/blob/main/packages/scalar.aspnetcore/README.md
        app.MapScalarApiReference(redocOptions =>
        {
            redocOptions.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
        });
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
    await Log.CloseAndFlushAsync();
}
