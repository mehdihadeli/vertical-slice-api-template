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
        app.UseCustomCors();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        app.UseAuthentication();
        app.UseAuthorization();

        // https://github.com/stevejgordon/CorrelationId
        app.UseCorrelationId();

        return Task.CompletedTask;
    }
}
