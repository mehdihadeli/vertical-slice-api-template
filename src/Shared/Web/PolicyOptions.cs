namespace Shared.Web;

public class PolicyOptions
{
    public int RetryCount { get; set; } = 3;
    public int BreakDuration { get; set; } = 30;
    public int TimeOutDuration { get; set; } = 15;
}
