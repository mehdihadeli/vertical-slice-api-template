using CorrelationId.DependencyInjection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Vertical.Slice.Template.Shared.Cache;
using Vertical.Slice.Template.Shared.Cache.Behaviours;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.EF;
using Vertical.Slice.Template.Shared.Logging;
using Vertical.Slice.Template.Shared.Resiliency;
using Vertical.Slice.Template.Shared.Swagger;
using Vertical.Slice.Template.Shared.Validation;
using Vertical.Slice.Template.Shared.Validation.Extensions;
using Vertical.Slice.Template.Shared.Web.Extensions;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.AddCustomSerilog();

        builder.Services.AddCore();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        builder.Services.AddAuthentication().AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.AddCustomCaching();

        // https://github.com/stevejgordon/CorrelationId
        builder.Services.AddDefaultCorrelationId();

        // #if EnableSwagger
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.AddCustomSwagger();
        // #endif

        builder.AddCustomVersioning();

        builder.AddCustomCors();

        builder.AddAppProblemDetails();

        // https://github.com/tonerdo/dotnet-env
        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CatalogsMetadata).Assembly));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamLoggingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(StreamCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));

        builder.Services.AddAutoMapper(typeof(CatalogsMetadata).Assembly);

        builder.Services.AddCustomValidators(typeof(CatalogsMetadata).Assembly);

        return builder;
    }
}
