using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Shared.HealthCheck;

public static class WebApplicationExtensions
{
    public static WebApplication MapCustomHealthChecks(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/health-checks#non-development-environments
            var healthChecks = app.MapGroup("");

            healthChecks.CacheOutput("HealthChecks").WithRequestTimeout("HealthChecks");

            // All health checks must pass for app to be
            // considered ready to accept traffic after starting
            healthChecks.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag
            // must pass for app to be considered alive
            healthChecks.MapHealthChecks("/alive", new() { Predicate = static r => r.Tags.Contains("live") });

            app.MapHealthChecks("ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

            // export health metrics in `/health/metrics` endpoint and should scrape in the Prometheus config file and `scrape_configs` beside of `/metrics` endpoint for application metrics
            app.UseHealthChecksPrometheusExporter(
                "/health/metrics",
                options =>
                {
                    options.ResultStatusCodes[HealthStatus.Unhealthy] = 200;
                }
            );
        }

        return app;
    }
}
