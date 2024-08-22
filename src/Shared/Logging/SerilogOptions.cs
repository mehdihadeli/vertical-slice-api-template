namespace Shared.Logging;

internal sealed class SerilogOptions
{
    public string? SeqUrl { get; set; }
    public bool UseConsole { get; set; } = true;
    public bool ExportLogsToOpenTelemetry { get; set; }
    public string? ElasticSearchUrl { get; set; }
    public string? GrafanaLokiUrl { get; set; }
    public bool UseElasticsearchJsonFormatter { get; set; }
    public string LogTemplate { get; set; } =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}";
    public string? LogPath { get; set; }
}
