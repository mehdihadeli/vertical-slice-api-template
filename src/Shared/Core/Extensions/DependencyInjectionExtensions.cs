using System.Reflection;
using Polly;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core.Domain.Events;
using Shared.Core.Paging;
using Shared.Core.Persistence.Extensions;
using Shared.Core.Reflection;
using Sieve.Services;

namespace Shared.Core.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var assemblies =
            assembliesToScan.Length != 0
                ? assembliesToScan
                : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventsAccessor, DomainEventAccessor>();

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

        services.AddPersistenceCore(assemblies);

        var policy = Policy.Handle<Exception>().RetryAsync(2);
        services.AddSingleton<AsyncPolicy>(policy);

        return services;
    }
}
