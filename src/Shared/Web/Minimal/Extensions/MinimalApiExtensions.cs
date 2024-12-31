using System.Reflection;
using LinqKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Abstractions.Web;
using Shared.Core.Extensions;
using Shared.Core.Reflection;

namespace Shared.Web.Minimal.Extensions;

public static class MinimalApiExtensions
{
    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        params Assembly[] scanAssemblies
    )
    {
        if (scanAssemblies.Length == 0)
        {
            // Find assemblies that reference the current assembly
            var referencingAssemblies = Assembly.GetExecutingAssembly().GetReferencingAssemblies();
            scanAssemblies = referencingAssemblies.ToArray();
        }

        applicationBuilder.Services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IMinimalEndpoint>()
                .WithLifetime(ServiceLifetime.Scoped)
        );

        return applicationBuilder.Services;
    }

    public static IServiceCollection AddMinimalEndpoints(
        this IServiceCollection services,
        params Assembly[] scanAssemblies
    )
    {
        if (scanAssemblies.Length == 0)
        {
            // Find assemblies that reference the current assembly
            var referencingAssemblies = Assembly.GetExecutingAssembly().GetReferencingAssemblies();
            scanAssemblies = referencingAssemblies.ToArray();
        }

        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IMinimalEndpoint>()
                .WithLifetime(ServiceLifetime.Scoped)
        );

        return services;
    }

    /// <summary>
    /// Map registered minimal apis.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static Microsoft.AspNetCore.Routing.IEndpointRouteBuilder MapMinimalEndpoints(
        this Microsoft.AspNetCore.Routing.IEndpointRouteBuilder builder
    )
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>().ToList();

        // https://github.com/dotnet/aspnet-api-versioning/commit/b789e7e980e83a7d2f82ce3b75235dee5e0724b4
        // changed from MapApiGroup to NewVersionedApi in v7.0.0
        var versionGroups = endpoints
            .GroupBy(x => x.GroupName)
            .ToDictionary(x => x.Key, c => builder.NewVersionedApi(c.Key).WithTags(c.Key));

        var versionSubGroups = endpoints
            .GroupBy(x => new
            {
                x.GroupName,
                x.PrefixRoute,
                x.Version,
            })
            .ToDictionary(
                x => x.Key,
                c => versionGroups[c.Key.GroupName].MapGroup(c.Key.PrefixRoute).HasApiVersion(c.Key.Version)
            );

        var endpointVersions = endpoints
            .GroupBy(x => new { x.GroupName, x.Version })
            .Select(x => new
            {
                Verion = x.Key.Version,
                x.Key.GroupName,
                Endpoints = x.Select(v => v),
            });

        foreach (var endpointVersion in endpointVersions)
        {
            var versionGroup = versionSubGroups.FirstOrDefault(x => x.Key.GroupName == endpointVersion.GroupName).Value;

            endpointVersion.Endpoints.ForEach(ep =>
            {
                ep.MapEndpoint(versionGroup);
            });
        }

        return builder;
    }
}
