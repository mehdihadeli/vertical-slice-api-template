using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Shared.Web.Extensions;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task UseInfrastructure(this WebApplication app)
    {
        app.UseCustomCors();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        app.UseAuthentication();
        app.UseAuthorization();

        // https://github.com/stevejgordon/CorrelationId
        app.UseCorrelationId();

        await app.MigrateDatabases();
    }
}
