using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Vertical.Slice.Template.Shared;

public class ApplicationInstrumentation
{
    public static Meter Meter = new Meter("Vertical.Slice.Template", "1.0.0");
    public static ActivitySource ActivitySource = new ActivitySource("Vertical.Slice.Template", "1.0.0");
}
