using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Shared.Cache;
using Shared.Cache.Behaviours;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.EF;
using Shared.Logging;
using Shared.Swagger;
using Shared.Validation;
using Shared.Web.Extensions;

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

        // #if EnableSwagger
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.AddCustomSwagger();
        // #endif

        builder.AddCustomVersioning();

        builder.AddAppProblemDetails();

        // https://github.com/tonerdo/dotnet-env
        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CatalogsMetadata).Assembly));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));

        builder.Services.AddAutoMapper(typeof(CatalogsMetadata).Assembly);

        builder.Services.AddValidatorsFromAssembly(typeof(CatalogsMetadata).Assembly);

        return builder;
    }
}
