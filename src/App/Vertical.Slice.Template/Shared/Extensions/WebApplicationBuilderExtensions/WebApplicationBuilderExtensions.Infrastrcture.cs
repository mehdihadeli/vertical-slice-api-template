using BuildingBlocks.Core.CQRS.Events;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.EF;
using Shared.Logging;
using Shared.Swagger;
using Shared.Validation;
using Shared.Web.Extensions;
using Sieve.Services;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Shared.Workers;
using AggregatesDomainEventsStore = Shared.Core.Domain.Events.AggregatesDomainEventsStore;

namespace Vertical.Slice.Template.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructures(this WebApplicationBuilder builder)
    {
        builder.AddCustomSerilog();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security
        builder.Services.AddAuthentication().AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.Services.AddHostedService<MigrationWorker>();
        builder.Services.AddHostedService<SeedWorker>();

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
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
        builder.Services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        builder.Services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();

        builder.Services.AddAutoMapper(x =>
        {
            x.AddProfile<ProductMappingProfiles>();
        });
        builder.Services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
        builder.Services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();

        builder.Services.AddValidatorsFromAssembly(typeof(CatalogsMetadata).Assembly);
        builder.Services.ScanAndRegisterDbExecutors();

        return builder;
    }
}
