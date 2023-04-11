using Microsoft.Extensions.Options;
using Shared.Swagger;
using Shared.Web.Extensions;
using Shared.Web.Middlewares;
using Shared.Web.Minimal.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vertical.Slice.Template.Api.Extensions.WebApplicationBuilderExtensions;
using Vertical.Slice.Template.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// #if EnableSwagger
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

// #endif
builder.AddInfrastructures();
builder.AddAppProblemDetails();
builder.AddCustomVersioning();

builder.AddModulesServices();

var app = builder.Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
// Does nothing if a response body has already been provided. when our next `DeveloperExceptionMiddleware` is written response for exception (in dev mode) when we back to `ExceptionHandlerMiddlewareImpl` because `context.Response.HasStarted` it doesn't do anything
// By default `ExceptionHandlerMiddlewareImpl` middleware register original exceptions with `IExceptionHandlerFeature` feature, we don't have this in `DeveloperExceptionPageMiddleware` and we should handle it with a middleware like `CaptureExceptionMiddleware`
// Just for handling exceptions in production mode
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errrors
    app.UseDeveloperExceptionPage();
    app.UseCaptureException();

    // #if EnableSwagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
    // #endif
}

await app.ConfigureModules();
app.MapModulesEndpoints();

app.Run();

// for tests
namespace Vertical.Slice.Template.Api
{
    public partial class Program { }
}
