namespace Vertical.Slice.Template.Shared.Resiliency.Options;

public class BulkheadPolicyOptions
{
    public int MaxParallelization { get; set; } = 10;
    public int MaxQueuingActions { get; set; } = 5;
}
