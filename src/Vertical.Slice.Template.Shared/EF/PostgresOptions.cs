namespace Vertical.Slice.Template.Shared.EF;

public class PostgresOptions
{
    public string ConnectionString { get; set; } = default!;
    public bool UseInMemory { get; set; }
    public string? MigrationAssembly { get; set; } = null!;
}
