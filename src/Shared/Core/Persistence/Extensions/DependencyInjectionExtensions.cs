using System.Reflection;
using Shared.Abstractions.Persistence;

namespace Shared.Core.Persistence.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddPersistenceCore(
        this IServiceCollection services,
        params Assembly[] assembliesToScan
    )
    {
        services.ScanAndRegisterDbExecutors(assembliesToScan);

        services.AddHostedService<SeedWorker>();
        services.AddScoped<IMigrationManager, MigrationManager>();

        return services;
    }
}
