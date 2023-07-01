namespace Vertical.Slice.Template.Shared.Resiliency.Options;

public class HttpClientOptions
{
    public virtual string BaseAddress { get; set; } = default!;
    public virtual int Timeout { get; set; } = 60;
}
