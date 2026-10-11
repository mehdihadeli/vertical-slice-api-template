using Microsoft.Extensions.Hosting;

namespace Vertical.Slice.Template.TestsShared.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task StartTestHostedServices(
        this IServiceProvider serviceProvider,
        IEnumerable<Type> hostedServiceTypes,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var hostedServiceType in hostedServiceTypes)
        {
            if (serviceProvider.GetService(hostedServiceType) is IHostedService hostedService)
                await hostedService.StartAsync(cancellationToken);
        }
    }

    public static async Task StopTestHostedServices(
        this IServiceProvider serviceProvider,
        IEnumerable<Type> hostedServiceTypes,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var hostedServiceType in hostedServiceTypes)
        {
            if (serviceProvider.GetService(hostedServiceType) is IHostedService hostedService)
                await hostedService.StopAsync(cancellationToken);
        }
    }
}
