using System.Reflection;
using System.Runtime.InteropServices;
using Shared.Abstractions.Persistence.Ef;
using Shared.Core.Reflection.Extensions;

namespace Shared.Core.Persistence.Extensions;

internal static class DbExecutorRegistrationsExtensions
{
    internal static IServiceCollection ScanAndRegisterDbExecutors(
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
}
