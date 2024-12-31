using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;
using Shared.Core.Extensions;

namespace Shared.Core.Persistence.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddPersistenceCore(this IServiceCollection services)
    {
        // Find assemblies that reference the current assembly
        var referencingAssemblies = Assembly.GetExecutingAssembly().GetReferencingAssemblies();
        var scanAssemblies = referencingAssemblies.ToArray();

        services.AddDataSeeders(scanAssemblies);
        services.AddDataMigrationSchemas(scanAssemblies);
        services.ScanAndRegisterDbExecutors(scanAssemblies);

        // registration order is important in the workers and running order is reverse
        AddMigration(services);
        services.AddHostedService<DataSeedWorker>();

        services.AddSingleton<IMigrationManager, MigrationManager>();
        services.AddSingleton<IDataSeederManager, DataSeederManager>();

        return services;
    }

    private static void AddMigration(IServiceCollection services)
    {
        // Enable migration tracing
        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(MigrationWorker.ActivitySourceName));
        services.AddHostedService<MigrationWorker>();
    }

    private static IServiceCollection ScanAndRegisterDbExecutors(
        this IServiceCollection services,
        params Assembly[] assembliesToScan
    )
    {
        var dbExecutors = assembliesToScan
            .SelectMany(x => x.GetLoadableTypes())
            .Where(t =>
                t!.IsClass
                && !t.IsAbstract
                && !t.IsGenericType
                && !t.IsInterface
                && t.GetConstructor(Type.EmptyTypes) != null
                && typeof(IDbExecutors).IsAssignableFrom(t)
            )
            .ToList();

        foreach (var dbExecutor in CollectionsMarshal.AsSpan(dbExecutors))
        {
            var instantiatedType = (IDbExecutors)Activator.CreateInstance(dbExecutor)!;
            instantiatedType.Register(services);
        }

        return services;
    }

    private static IServiceCollection AddDataSeeders(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IDataSeeder>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }

    private static IServiceCollection AddDataMigrationSchemas(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies
    )
    {
        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IMigrationSchema>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }
}
