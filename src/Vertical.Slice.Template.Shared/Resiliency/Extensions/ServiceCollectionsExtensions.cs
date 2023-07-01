using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Vertical.Slice.Template.Shared.Core.Extensions.ServiceCollectionsExtensions;
using Vertical.Slice.Template.Shared.Resiliency.Options;

namespace Vertical.Slice.Template.Shared.Resiliency.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddCustomPolicyRegistry(this IServiceCollection services)
    {
        services.AddValidatedOptions<PolicyOptions>();

        // https://rehansaeed.com/optimally-configuring-asp-net-core-httpclientfactory/
        // https://github.com/App-vNext/Polly/wiki/PolicyRegistry
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
        services.AddPolicyRegistry(
            (sp, registry) =>
            {
                var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>().Value;

                var retryPolicy =
                // HttpPolicyExtensions.HandleTransientHttpError()
                Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                    .WaitAndRetryAsync(
                        policyOptions.RetryPolicyOptions.Count,
                        retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(policyOptions.RetryPolicyOptions.BackoffPower, retryAttempt)),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            if (outcome.Result != null)
                            {
                                // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory#configuring-httpclientfactory-policies-to-use-an-iloggert-from-the-call-site
                                context
                                    .GetLogger()
                                    ?.LogWarning(
                                        "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                                        outcome.Result.StatusCode,
                                        timespan,
                                        retryAttempt
                                    );
                            }
                            else
                            {
                                context
                                    .GetLogger()
                                    ?.LogWarning(
                                        "Request failed because network failure. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                                        timespan,
                                        retryAttempt
                                    );
                            }
                        }
                    );

                var circuitBreakerPolicy = Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .CircuitBreakerAsync(
                        policyOptions.CircuitBreakerPolicyOptions.ExceptionsAllowedBeforeBreaking,
                        TimeSpan.FromSeconds(policyOptions.CircuitBreakerPolicyOptions.DurationOfBreak),
                        onBreak: (ex, breakDuration, context) =>
                        {
                            context
                                .GetLogger()
                                ?.LogWarning(
                                    "Circuit breaker tripped due to {ExceptionName}. Duration of break: {BreakDuration}",
                                    ex.GetType().Name,
                                    breakDuration
                                );
                            throw new BrokenCircuitException("Service inoperative. Please try again later");
                        },
                        onReset: context =>
                        {
                            context.GetLogger()?.LogWarning("Circuit breaker reset");
                        }
                    );

                // Timeout for an individual try
                var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                    TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds)
                );

                var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
                    policyOptions.BulkheadPolicyOptions.MaxParallelization,
                    policyOptions.BulkheadPolicyOptions.MaxQueuingActions
                );

                registry.Add(PolicyNames.Retry, retryPolicy);
                registry.Add(PolicyNames.CircuitBreaker, circuitBreakerPolicy);
                registry.Add(PolicyNames.Timeout, timeoutPolicy); // We place the timeoutPolicy inside the retryPolicy, to make it time out each try.
                registry.Add(PolicyNames.Bulkhead, bulkheadPolicy);
            }
        );

        return services;
    }

    public static IServiceCollection AddCustomHttpClient<TClient, TImplementation, TClientOptions>(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient>? configureClient = null
    )
        where TClient : class
        where TImplementation : class, TClient
        where TClientOptions : HttpClientOptions, new()
    {
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
        // https://github.com/App-vNext/Polly/wiki/PolicyRegistry
        services.AddValidatedOptions<TClientOptions>();

        services.RegisterCustomHandlers();

        services
            .AddHttpClient<TClient, TImplementation>()
            .ConfigureHttpClient(
                (serviceProvider, httpClient) =>
                {
                    var httpClientOptions = serviceProvider.GetRequiredService<IOptions<TClientOptions>>().Value;
                    httpClient.BaseAddress = new Uri(httpClientOptions.BaseAddress);
                    httpClient.Timeout = TimeSpan.FromSeconds(httpClientOptions.Timeout);

                    configureClient?.Invoke(serviceProvider, httpClient);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
            .AddPolicyHandlerFromRegistry(PolicyNames.Retry)
            .AddPolicyHandlerFromRegistry(PolicyNames.CircuitBreaker)
            .AddPolicyHandlerFromRegistry(PolicyNames.Timeout)
            .AddPolicyHandlerFromRegistry(PolicyNames.Bulkhead)
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
        // .AddHttpMessageHandler(sp =>
        // {
        //     return new CorrelationIdDelegatingHandler(
        //         sp.GetService<ICorrelationContextAccessor>(),
        //         sp.GetService<IOptions<CorrelationIdOptions>>()
        //     );
        // };

        return services;
    }

    public static IServiceCollection AddCustomHttpClient<TClient, TClientOptions>(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient>? configureClient = null
    )
        where TClient : class
        where TClientOptions : HttpClientOptions, new()
    {
        return services.AddCustomHttpClient<TClient, TClient, TClientOptions>(configureClient);
    }

    public static IServiceCollection AddCustomHttpClient<TClientOptions>(
        this IServiceCollection services,
        string clientName,
        Action<IServiceProvider, HttpClient>? configureClient = null
    )
        where TClientOptions : HttpClientOptions, new()
    {
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory#step-2-configure-a-client-with-polly-policies-in-startup
        services.AddValidatedOptions<TClientOptions>();

        services.RegisterCustomHandlers();

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#named-clients
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#createclient
        services
            .AddHttpClient(clientName)
            .ConfigureHttpClient(
                (sp, httpClient) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    httpClient.BaseAddress = new Uri(httpClientOptions.BaseAddress);
                    httpClient.Timeout = TimeSpan.FromSeconds(httpClientOptions.Timeout);

                    configureClient?.Invoke(sp, httpClient);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
            .AddPolicyHandlerFromRegistry(PolicyNames.Retry)
            .AddPolicyHandlerFromRegistry(PolicyNames.CircuitBreaker)
            .AddPolicyHandlerFromRegistry(PolicyNames.Timeout)
            .AddPolicyHandlerFromRegistry(PolicyNames.Bulkhead)
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

        return services;
    }

    private static void RegisterCustomHandlers(this IServiceCollection services)
    {
        // for preventing registering duplicate dependencies we should use `TrySingleton`
        services.TryAddTransient<CorrelationIdDelegatingHandler>();
    }
}
