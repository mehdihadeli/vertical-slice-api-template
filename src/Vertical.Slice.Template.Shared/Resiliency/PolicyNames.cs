namespace Vertical.Slice.Template.Shared.Resiliency;

public static class PolicyNames
{
    public const string Retry = "RetryPolicy";
    public const string CircuitBreaker = "CircuitBreakerPolicy";
    public const string Timeout = "TimeoutPolicy";
    public const string Bulkhead = "BulkheadPolicy";
}
