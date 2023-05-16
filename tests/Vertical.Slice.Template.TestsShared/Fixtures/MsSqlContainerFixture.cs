using Microsoft.Data.SqlClient;
using Respawn;
using Testcontainers.MsSql;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Xunit.Sdk;

namespace Vertical.Slice.Template.TestsShared.Fixtures;

public class MsSqlContainerFixture : IAsyncLifetime
{
    private readonly IMessageSink _messageSink;
    public MsSqlContainerOptions MsSqlContainerOptions { get; }
    public MsSqlContainer Container { get; }
    public int HostPort => Container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);
    public int TcpContainerPort => MsSqlBuilder.MsSqlPort;

    public MsSqlContainerFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        MsSqlContainerOptions = Helpers.ConfigurationHelper.BindOptions<MsSqlContainerOptions>();
        MsSqlContainerOptions.NotBeNull();

        var postgresContainerBuilder = new MsSqlBuilder()
            .WithImage(MsSqlContainerOptions.ImageName)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithCleanUp(true)
            .WithName(MsSqlContainerOptions.Name);

        Container = postgresContainerBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        _messageSink.OnMessage(
            new DiagnosticMessage(
                $"MsSql fixture started on Host port {HostPort} and container tcp port {TcpContainerPort}..."
            )
        );
    }

    public async Task ResetDbAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(Container.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        var checkpoint = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions { DbAdapter = DbAdapter.SqlServer }
        );

        await checkpoint.ResetAsync(connection)!;
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync(); //important for the event to cleanup to be fired!
        _messageSink.OnMessage(new DiagnosticMessage("Postgres fixture stopped."));
    }
}

public sealed class MsSqlContainerOptions
{
    public string Name { get; set; } = "sql_" + Guid.NewGuid();
    public string ImageName { get; set; } = "mcr.microsoft.com/mssql/server";
}
