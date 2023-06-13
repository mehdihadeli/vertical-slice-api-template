using System.Reflection;
using Sieve.Services;
using Vertical.Slice.Template.Shared.Abstractions.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Abstractions.Ef.Repository;
using Vertical.Slice.Template.Shared.Core.Domain.Events;
using Vertical.Slice.Template.Shared.Core.Ef;
using Vertical.Slice.Template.Shared.Core.Paging;

namespace Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;

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
