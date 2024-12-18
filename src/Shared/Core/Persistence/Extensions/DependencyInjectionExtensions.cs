using System.Reflection;
using System.Runtime.InteropServices;
using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;
using Shared.Core.Reflection.Extensions;

namespace Shared.Core.Persistence.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddPersistenceCore(this IServiceCollection services)
    {
        var assemblies = AppDomain
            .CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic) // Exclude dynamic assemblies
            .ToArray();

        services.AddDataSeeders(assemblies);
        services.AddDataMigrationSchemas(assemblies);
        services.ScanAndRegisterDbExecutors(assemblies);

        // registration order is important in the workers and running order is reverse
        services.AddHostedService<MigrationWorker>();
        services.AddHostedService<DataSeedWorker>();

        services.AddSingleton<IMigrationManager, MigrationManager>();
        services.AddSingleton<IDataSeederManager, DataSeederManager>();

        return services;
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
