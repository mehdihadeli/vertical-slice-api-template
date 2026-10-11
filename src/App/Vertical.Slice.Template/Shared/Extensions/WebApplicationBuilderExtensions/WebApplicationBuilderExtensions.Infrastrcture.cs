using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Shared.Abstractions.Persistence.Ef.Repository;
using Shared.Cache;
using Shared.Cache.Behaviors;
using Shared.Core.Extensions;
using Shared.Core.Messaging;
using Shared.EF;
using Shared.HealthCheck;
using Shared.Logging;
using Shared.Messaging.Wolverine;
using Shared.Diagnostics;
using Shared.Diagnostics.Behaviors;
using Shared.RateLimit;
using Shared.Resiliency;
using Shared.Validation;
using Shared.Validation.Extensions;
using Shared.Web.Extensions;
using Shared.Web.Extensions.WebApplicationBuilderExtensions;
using Shared.Web.HeaderPropagation;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.AddAppProblemDetails();

        builder.Services.AddCore();

        builder.AddWolverineEventBus();

        // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/health-checks#non-development-environments
        builder.Services.AddRequestTimeouts();
        builder.Services.AddOutputCache();

        builder.AddCustomDiagnostics();

        builder.AddCustomRateLimit();

        if (builder.Environment.IsTest() == false)
        {
            builder.AddCustomHealthCheck(healthChecksBuilder =>
            {
                var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>(nameof(PostgresOptions));
                postgresOptions.NotBeNull();

                healthChecksBuilder.AddNpgSql(
                    postgresOptions.ConnectionString,
                    name: "CatalogsService-Postgres-Check",
                    tags: ["live", "ready"]
                );
            });
        }

        builder.AddCustomResiliency(false);

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        builder.Services.AddAuthentication().AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.AddCustomCaching();

        // https://aurelien-riv.github.io/aspnetcore/2022/11/09/aspnet-grafana-loki-telemetry-microservice-correlation.html
        // https://www.nuget.org/packages/Microsoft.AspNetCore.HeaderPropagation
        // https://gist.github.com/davidfowl/c34633f1ddc519f030a1c0c5abe8e867
        // https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/HeaderPropagation/test/HeaderPropagationIntegrationTest.cs
        builder.Services.AddHeaderPropagation(options =>
        {
            options.Headers.Add(MessagingHeaders.CorrelationId);
            options.Headers.Add(MessagingHeaders.CausationId);
        });

        // for applying message handler to all http clients
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IHttpMessageHandlerBuilderFilter,
                HeaderPropagationMessageHandlerBuilderFilter
            >()
        );

        builder.AddCustomVersioning();

        builder.AddCustomCors();

        // https://github.com/tonerdo/dotnet-env
        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.Services.AddHttpContextAccessor();

        // https://github.com/martinothamar/Mediator
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Transient;
            options.Namespace = "Vertical.Slice.Template";
        });

        builder.Services.AddCustomValidators(typeof(CatalogsMetadata).Assembly);

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamLoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ObservabilityPipelineBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(GenericRepository<>));

        // External Clients
        builder.AddCustomHttpClients();

        return builder;
    }
}
