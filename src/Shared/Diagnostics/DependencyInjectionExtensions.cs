using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Observability;
using Shared.Core.Extensions;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Diagnostics.CoreDiagnostics.Commands;
using Shared.Diagnostics.CoreDiagnostics.Query;

namespace Shared.Diagnostics;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddCustomDiagnostics(this WebApplicationBuilder builder)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;

        builder.Services.AddSingleton<IDiagnosticsProvider, DiagnosticsProvider>();
        builder.AddCoreDiagnostics();

        builder.Services.AddConfigurationOptions<DiagnosticsOptions>(nameof(DiagnosticsOptions));
        var observabilityOptions = builder.Configuration.BindOptions<DiagnosticsOptions>();

        ArgumentException.ThrowIfNullOrEmpty(observabilityOptions.InstrumentationName);
        ObservabilityConstant.InstrumentationName = observabilityOptions.InstrumentationName;

        return builder;
    }

    private static WebApplicationBuilder AddCoreDiagnostics(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<CommandHandlerActivity>();
        builder.Services.AddTransient<CommandHandlerMetrics>();
        builder.Services.AddTransient<QueryHandlerActivity>();
        builder.Services.AddTransient<QueryHandlerMetrics>();

        return builder;
    }
}
