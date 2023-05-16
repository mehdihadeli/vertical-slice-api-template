using Microsoft.Extensions.Logging;
using Polly;

namespace Vertical.Slice.Template.Shared.Resiliency.Extensions;

// https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory#configuring-httpclientfactory-policies-to-use-an-iloggert-from-the-call-site
public static class PollyContextExtensions
{
    private const string LoggerKey = "ILogger";

    public static Context WithLogger<T>(this Context context, ILogger logger)
    {
        context[LoggerKey] = logger;
        return context;
    }

    public static ILogger? GetLogger(this Context context)
    {
        if (context.TryGetValue(LoggerKey, out object logger))
        {
            return logger as ILogger;
        }

        return null;
    }
}
