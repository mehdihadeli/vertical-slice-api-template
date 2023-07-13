using System.Reflection;
using Serilog;
using Serilog.Events;
using Vertical.Slice.Template;
using Vertical.Slice.Template.Shared;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;
using Vertical.Slice.Template.Shared.Logging;
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

    builder.AddCustomSerilog();

    builder.AddAppProblemDetails();

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

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
    // Does nothing if a response body has already been provided. when our next `DeveloperExceptionMiddleware` is written response for exception (in dev mode) when we back to `ExceptionHandlerMiddlewareImpl` because `context.Response.HasStarted` it doesn't do anything
    // By default `ExceptionHandlerMiddlewareImpl` middleware register original exceptions with `IExceptionHandlerFeature` feature, we don't have this in `DeveloperExceptionPageMiddleware` and we should handle it with a middleware like `CaptureExceptionMiddleware`
    // Just for handling exceptions in production mode
    // https://github.com/dotnet/aspnetcore/pull/26567
    app.UseExceptionHandler(options: new ExceptionHandlerOptions { AllowStatusCode404Response = true });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("test"))
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errrors
        app.UseDeveloperExceptionPage();

        // https://github.com/dotnet/aspnetcore/issues/4765
        // https://github.com/dotnet/aspnetcore/pull/47760
        // .net 8 will add `IExceptionHandlerFeature`in `DisplayExceptionContent` and `SetExceptionHandlerFeatures` methods `DeveloperExceptionPageMiddlewareImpl` class, exact functionality of CaptureException
        // bet before .net 8 preview 5 we should add `IExceptionHandlerFeature` manually with our `UseCaptureException`
        // app.UseCaptureException();
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
