using Microsoft.AspNetCore.Builder;

namespace Shared.Web.Middlewares.CaptureExceptionMiddleware;

public static class CaptureExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCaptureException(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.Properties["analysis.NextMiddlewareName"] = "Shared.Web.Middlewares.CaptureExceptionMiddleware";
        return app.UseMiddleware<CaptureExceptionMiddlewareImp>();
    }
}
