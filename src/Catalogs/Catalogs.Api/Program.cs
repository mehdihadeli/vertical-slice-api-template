using Catalogs.Shared.Extensions;
using ECommerce.Services.Catalogs.Shared.Extensions.WebApplicationBuilderExtensions;
using Shared.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructures();
builder.AddCustomProblemDetails();
builder.AddCustomVersioning();

builder.AddModulesServices();

var app = builder.Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errrors
    app.UseDeveloperExceptionPage();
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
}

await app.ConfigureModules();
app.MapModulesEndpoints();

app.Run();

// for tests
public partial class Program { }
