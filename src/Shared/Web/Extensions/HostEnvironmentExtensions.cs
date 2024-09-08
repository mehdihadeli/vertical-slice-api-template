using Microsoft.Extensions.Hosting;

namespace Shared.Web.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment env) => env.IsEnvironment("Test");

    public static bool IsDependencyTest(this IHostEnvironment env) => env.IsEnvironment("DependencyTest");

    public static bool IsDocker(this IHostEnvironment env) => env.IsEnvironment("Docker");
}
