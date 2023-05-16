namespace Vertical.Slice.Template.Shared.Resiliency.Options;

public class RetryPolicyOptions
{
    public int Count { get; set; } = 3;

    public int BackoffPower { get; set; } = 2;
}
