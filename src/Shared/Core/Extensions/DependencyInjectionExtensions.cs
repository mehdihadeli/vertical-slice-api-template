using System.Reflection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using Shared.Abstractions.Core.Domain.Events;
using Shared.Core.Domain.Events;
using Shared.Core.Extensions.ServiceCollectionsExtensions;
using Shared.Core.Paging;
using Shared.Core.Persistence.Extensions;
using Shared.Core.Reflection;
using Shared.Resiliency.Options;
using Sieve.Services;

namespace Shared.Core.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        var assemblies =
            assembliesToScan.Length != 0
                ? assembliesToScan
                : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventsAccessor, DomainEventAccessor>();

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

        services.AddPersistenceCore();

        services.AddValidatedOptions<PolicyOptions>(nameof(PolicyOptions));

        // `AsyncPolicyWrap<HttpResponseMessage>` can be injected in clients and can be reused.
        services.AddSingleton<AsyncPolicyWrap<HttpResponseMessage>>(sp =>
        {
            var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>().Value.NotBeNull();

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(policyOptions.RetryPolicyOptions.Count);

            // HttpClient itself will still enforce its own timeout, which is 100 seconds by default. To fix this issue, you need to set the HttpClient.Timeout property to match or exceed the timeout configured in Polly's policy.
            var timeoutPolicy = Policy.TimeoutAsync(
                policyOptions.TimeoutPolicyOptions.TimeoutInSeconds,
                TimeoutStrategy.Pessimistic
            );

            // at any given time there will 3 parallel requests execution for specific service call and another 6 requests for other services can be in the queue. So that if the response from customer service is delayed or blocked then we don’t use too many resources
            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(3, 6);

            // https://github.com/App-vNext/Polly#handing-return-values-and-policytresult
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    policyOptions.RetryPolicyOptions.Count + 1,
                    TimeSpan.FromSeconds(policyOptions.CircuitBreakerPolicyOptions.DurationOfBreak)
                );

            var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, bulkheadPolicy);

            var finalPolicy = combinedPolicy.WrapAsync(timeoutPolicy);

            return finalPolicy;
        });

        return services;
    }
}
