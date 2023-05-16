using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Vertical.Slice.Template.Shared.Resiliency.Options;

namespace Vertical.Slice.Template.Shared.Resiliency.Extensions;

// https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddRetryHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (serviceProvider, httpRequestMessage) =>
            {
                var policyOptions = serviceProvider.GetRequiredService<IOptions<PolicyOptions>>().Value;

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
                    )
                    .WithPolicyKey(PolicyNames.Retry);

                return retryPolicy;
            }
        );
    }

    public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (serviceProvider, httpRequestMessage) =>
            {
                var policyOptions = serviceProvider.GetRequiredService<IOptions<PolicyOptions>>().Value;
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
                    )
                    .WithPolicyKey(PolicyNames.CircuitBreaker);

                return circuitBreakerPolicy;
            }
        );
    }

    public static IHttpClientBuilder AddTimeoutHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (serviceProvider, httpRequestMessage) =>
            {
                var policyOptions = serviceProvider.GetRequiredService<IOptions<PolicyOptions>>().Value;

                // Timeout for an individual try
                var timeoutPolicy = Policy
                    .TimeoutAsync<HttpResponseMessage>(
                        TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds)
                    )
                    .WithPolicyKey(PolicyNames.Timeout);

                return timeoutPolicy;
            }
        );
    }

    public static IHttpClientBuilder AddBulkheadHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (serviceProvider, httpRequestMessage) =>
            {
                var policyOptions = serviceProvider.GetRequiredService<IOptions<PolicyOptions>>().Value;

                var bulkheadPolicy = Policy
                    .BulkheadAsync<HttpResponseMessage>(
                        policyOptions.BulkheadPolicyOptions.MaxParallelization,
                        policyOptions.BulkheadPolicyOptions.MaxQueuingActions
                    )
                    .WithPolicyKey(PolicyNames.Bulkhead);

                return bulkheadPolicy;
            }
        );
    }
}
