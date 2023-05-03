using System.Reflection;
using Microsoft.AspNetCore.Http;
using Scrutor;
using Shared.Abstractions.Web;
using Shared.Core.Reflection;
using Shared.Web.Extensions.ServiceCollection;

namespace Shared.Web.ProblemDetail;

// https://www.strathweb.com/2022/08/problem-details-responses-everywhere-with-asp-net-core-and-net-7/
public static class RegistrationExtensions
{
    public static IServiceCollection AddCustomProblemDetails(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure,
        params Assembly[] scanAssemblies
    )
    {
        services.AddProblemDetails(configure);
        services.ReplaceSingleton<IProblemDetailsService, ProblemDetailsService>();
        // services.TryAddSingleton<IProblemDetailMapper, DefaultProblemDetailMapper>();
        RegisterAllMappers(services, scanAssemblies);

        return services;
    }

    private static void RegisterAllMappers(IServiceCollection services, Assembly[] scanAssemblies)
    {
        var assemblies = scanAssemblies.Any()
            ? scanAssemblies
            : ReflectionUtilities
                .GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(ReflectionUtilities.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        services.Scan(
            scan =>
                scan.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo(typeof(IProblemDetailMapper)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .As<IProblemDetailMapper>()
                    .WithLifetime(ServiceLifetime.Singleton)
        );
    }
}
