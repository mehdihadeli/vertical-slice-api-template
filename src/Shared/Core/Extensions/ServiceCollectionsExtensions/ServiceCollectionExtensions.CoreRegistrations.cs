using System.Reflection;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core.Domain.Events;
using Shared.Core.Paging;
using Sieve.Services;

namespace Shared.Core.Extensions.ServiceCollectionsExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var scanAssemblies = assembliesToScan.Any() ? assembliesToScan : new[] { Assembly.GetCallingAssembly() };
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventsAccessor, DomainEventAccessor>();

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
        services.ScanAndRegisterDbExecutors(scanAssemblies);

        return services;
    }
}
