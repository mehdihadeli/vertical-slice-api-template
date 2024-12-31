using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core.Domain.Events;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Core.Paging;
using Shared.Core.Persistence.Extensions;
using Shared.Core.Reflection;
using Shared.Resiliency.Options;
using Sieve.Services;

namespace Shared.Core.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventsAccessor, DomainEventAccessor>();

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

        services.AddPersistenceCore();

        return services;
    }
}
