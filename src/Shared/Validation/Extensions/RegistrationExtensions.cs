using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Core.Extensions;

namespace Shared.Validation.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddCustomValidators(this IServiceCollection services, Assembly assembly)
    {
        // TODO: problem with registering internal validators
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }
}
