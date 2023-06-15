using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Vertical.Slice.Template.Shared.Web.Extensions;
using Vertical.Slice.Template.Shared.Web.ProblemDetail.Middlewares.CaptureExceptionMiddleware;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static Task UseInfrastructure(this WebApplication app)
    {
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
            app.UseCaptureException();
        }

        // this middleware should be first middleware
        // request logging just log in information level and above as default
        app.UseSerilogRequestLogging();

        app.UseCustomCors();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        app.UseAuthentication();
        app.UseAuthorization();

        // https://github.com/stevejgordon/CorrelationId
        app.UseCorrelationId();

        return Task.CompletedTask;
    }
}
