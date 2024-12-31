using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Observability;

public static class WebApplicationExtensions
{
    public static WebApplication UseObservability(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ObservabilityOptions>>().Value;

        app.Use(
            async (context, next) =>
            {
                var metricsFeature = context.Features.Get<IHttpMetricsTagsFeature>();
                if (metricsFeature != null && context.Request.Path is { Value: "/metrics" or "/health" })
                {
                    metricsFeature.MetricsDisabled = true;
                }

                await next(context);
            }
        );

        if (options.UsePrometheusOTLPMetrics)
        {
            // export application metrics in `/metrics` endpoint and should scrape in the Prometheus config file and `scrape_configs`
            app.MapPrometheusScrapingEndpoint().AllowAnonymous();
        }

        return app;
    }
}
