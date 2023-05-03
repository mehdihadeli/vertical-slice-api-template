using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Shared.Web.ProblemDetail;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomProblemDetails(problemDetailsOptions =>
        {
            // customization problem details should go here
            problemDetailsOptions.CustomizeProblemDetails = problemDetailContext =>
            {
                // with help of capture exception middleware for capturing actual exception
                if (problemDetailContext.HttpContext.Features.Get<IExceptionHandlerFeature>() is { } exceptionFeature)
                { }
            };
        });

        return builder;
    }
}
