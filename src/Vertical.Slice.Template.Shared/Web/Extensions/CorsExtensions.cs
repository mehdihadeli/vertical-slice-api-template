using Microsoft.AspNetCore.Builder;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;

namespace Vertical.Slice.Template.Shared.Web.Extensions;

// https://learn.microsoft.com/en-us/aspnet/core/security/cors
public static class Extensions
{
    private const string CorsPolicyName = "AllowSpecificOrigins";

    public static WebApplicationBuilder AddCustomCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatedOptions<CorsOptions>();

        builder.Services.AddCors();

        return builder;
    }

    public static WebApplication UseCustomCors(this WebApplication app)
    {
        var options = app.Services.GetService<CorsOptions>();
        app.UseCors(p =>
        {
            if (options?.AllowedUrls is { } && options.AllowedUrls.Any())
            {
                p.WithOrigins(options.AllowedUrls.ToArray());
            }
            else
            {
                p.AllowAnyOrigin();
            }

            p.AllowAnyMethod();
            p.AllowAnyHeader();
        });
        return app;
    }
}
