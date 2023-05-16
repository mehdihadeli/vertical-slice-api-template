using System.Reflection;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Core.Reflection.Extensions;

namespace Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection ScanAndRegisterDbExecutors(
        this IServiceCollection services,
        IList<Assembly> assembliesToScan
    )
    {
        var dbExecutors = assembliesToScan
            .SelectMany(x => x.GetLoadableTypes())
            .Where(
                t =>
                    t!.IsClass
                    && !t.IsAbstract
                    && !t.IsGenericType
                    && !t.IsInterface
                    && t.GetConstructor(Type.EmptyTypes) != null
                    && typeof(IDbExecutors).IsAssignableFrom(t)
            )
            .ToList();

        foreach (var dbExecutor in dbExecutors)
        {
            var instantiatedType = (IDbExecutors)Activator.CreateInstance(dbExecutor)!;
            instantiatedType.Register(services);
        }

        return services;
    }
}
