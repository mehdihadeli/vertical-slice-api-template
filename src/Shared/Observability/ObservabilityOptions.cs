namespace Shared.Observability;

public class ObservabilityOptions
{
    public string InstrumentationName { get; set; } = default!;
    public string? ServiceName { get; set; }
    public bool MetricsEnabled { get; set; } = true;
    public bool TracingEnabled { get; set; } = true;
    public bool LoggingEnabled { get; set; } = true;
    public bool UsePrometheusOTLPMetrics { get; set; } = true;
    public bool UseOTLPExporter { get; set; } = true;
    public bool UseAspireOTLPExporter { get; set; } = true;
    public bool UseGrafanaExporter { get; set; }
    public bool UseConsoleExporter { get; set; }
    public bool UseJaegerExporter { get; set; }
    public bool UseZipkinExporter { get; set; }
    public ZipkinOptions ZipkinOptions { get; set; } = default!;
    public JaegerOptions JaegerOptions { get; set; } = default!;
    public OTLPOptions OTLPOptions { get; set; } = default!;
    public AspireDashboardOTLPOptions AspireDashboardOTLPOptions { get; set; } = default!;
}

public class ZipkinOptions
{
    public string ExporterEndpoint { get; set; } = default!;
    public string OTLPExporterEndpoint { get; set; } = default!;
}

public class JaegerOptions
{
    public string ExporterEndpoint { get; set; } = default!;
    public string OTLPExporterEndpoint { get; set; } = default!;
}

public class OTLPOptions
{
    public string OTLPExporterEndpoint { get; set; } = default!;
}

public class AspireDashboardOTLPOptions
{
    public string OTLPExporterEndpoint { get; set; } = default!;
}
