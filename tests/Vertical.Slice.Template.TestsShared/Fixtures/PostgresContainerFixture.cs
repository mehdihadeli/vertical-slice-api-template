using Dapper;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.TestsShared.Helpers;
using Xunit.Sdk;

namespace Vertical.Slice.Template.TestsShared.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly IMessageSink _messageSink;
    public PostgresContainerOptions PostgresContainerOptions { get; }
    public PostgreSqlContainer Container { get; }
    public int HostPort => Container.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);
    public int TcpContainerPort => PostgreSqlBuilder.PostgreSqlPort;

    public PostgresContainerFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        PostgresContainerOptions = ConfigurationHelper.BindOptions<PostgresContainerOptions>();
        PostgresContainerOptions.NotBeNull();

        var postgresContainerBuilder = new PostgreSqlBuilder()
            .WithDatabase(PostgresContainerOptions.DatabaseName)
            .WithCleanUp(true)
            .WithName(PostgresContainerOptions.Name)
            .WithImage(PostgresContainerOptions.ImageName);

        Container = postgresContainerBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        _messageSink.OnMessage(
            new DiagnosticMessage(
                $"Postgres fixture started on Host port {HostPort} and container tcp port {TcpContainerPort}..."
            )
        );
    }

    public async Task ResetDbAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(Container.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var checkpoint = await Respawner.CreateAsync(
                connection,
                new RespawnerOptions { DbAdapter = DbAdapter.Postgres }
            );

            //TODO: should update to latest version after release a new version
            // https://github.com/jbogard/Respawn/issues/108
            // https://github.com/jbogard/Respawn/pull/115 - fixed
            // waiting for new nuget version of respawn, current is 6.
            await checkpoint.ResetAsync(connection)!;
        }
        catch (Exception e)
        {
            _messageSink.OnMessage(new DiagnosticMessage(e.Message));
        }
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync(); //important for the event to cleanup to be fired!
        _messageSink.OnMessage(new DiagnosticMessage("Postgres fixture stopped."));
    }
}

public sealed class PostgresContainerOptions
{
    public string Name { get; set; } = "postgres_" + Guid.NewGuid();
    public string ImageName { get; set; } = "postgres:latest";
    public string DatabaseName { get; set; } = "test_db";
}
