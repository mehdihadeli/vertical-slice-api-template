using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.HealthCheck;

// https://dev.to/dbolotov/observability-with-grafana-cloud-and-opentelemetry-in-net-microservices-448c

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddCustomHealthCheck(
        this WebApplicationBuilder builder,
        Action<IHealthChecksBuilder>? healthChecksBuilder = null
    )
    {
        // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/health-checks#non-development-environments
        builder.Services.AddRequestTimeouts(configure: static timeouts =>
            timeouts.AddPolicy("HealthChecks", TimeSpan.FromSeconds(5))
        );

        builder.Services.AddOutputCache(configureOptions: static caching =>
            caching.AddPolicy("HealthChecks", build: static policy => policy.Expire(TimeSpan.FromSeconds(10)))
        );

        var healCheckBuilder = builder
            .Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddDiskStorageHealthCheck(_ => { }, tags: new[] { "live", "ready" })
            .AddPingHealthCheck(_ => { }, tags: new[] { "live", "ready" })
            .AddPrivateMemoryHealthCheck(512 * 1024 * 1024, tags: new[] { "live", "ready" })
            .AddDnsResolveHealthCheck(_ => { }, tags: new[] { "live", "ready" });

        healthChecksBuilder?.Invoke(healCheckBuilder);

        return builder;
    }
}
