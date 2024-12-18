using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.OpenApi.AspnetOpenApi.Extensions;
using Shared.Web.Extensions;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static Task UseInfrastructure(this WebApplication app)
    {
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

        app.UseCustomCors();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        app.UseAuthentication();
        app.UseAuthorization();

        // https://github.com/stevejgordon/CorrelationId
        app.UseCorrelationId();

        // #if EnableSwagger
        if (app.Environment.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }
        // #endif

        return Task.CompletedTask;
    }
}
