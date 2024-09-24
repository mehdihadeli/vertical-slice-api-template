using CorrelationId.DependencyInjection;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Shared.Abstractions.Persistence.Ef.Repository;
using Shared.Cache;
using Shared.Cache.Behaviours;
using Shared.Core.Extensions;
using Shared.EF;
using Shared.Logging;
using Shared.Swagger;
using Shared.Validation;
using Shared.Validation.Extensions;
using Shared.Web.Extensions;
using Shared.Web.Extensions.WebApplicationBuilderExtensions;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.Services.AddCore();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        builder.Services.AddAuthentication().AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.AddCustomEasyCaching();

        // https://github.com/stevejgordon/CorrelationId
        builder.Services.AddDefaultCorrelationId();

        // #if EnableSwagger
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.AddCustomSwagger();
        // #endif

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

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamLoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));

        builder.Services.AddCustomValidators(typeof(CatalogsMetadata).Assembly);

        builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(GenericRepository<>));

        // External Clients
        builder.AddCustomHttpClients();

        return builder;
    }
}
