using Microsoft.AspNetCore.Builder;

namespace Vertical.Slice.Template.Shared.Web.ProblemDetail.Middlewares.CaptureExceptionMiddleware;

public static class CaptureExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCaptureException(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.Properties["analysis.NextMiddlewareName"] =
            "Vertical.Slice.Template.Shared.Web.Middlewares.CaptureExceptionMiddleware";
        return app.UseMiddleware<CaptureExceptionMiddlewareImp>();
    }
}
