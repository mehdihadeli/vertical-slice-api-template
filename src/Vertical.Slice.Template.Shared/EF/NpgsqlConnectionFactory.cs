using System.Data;
using System.Data.Common;
using Npgsql;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Core.Extensions;

namespace Vertical.Slice.Template.Shared.EF;

public class NpgsqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    private DbConnection? _connection;

    public NpgsqlConnectionFactory(string? connectionString)
    {
        connectionString.NotBeNullOrWhiteSpace();
        _connectionString = connectionString;
    }

    public async Task<DbConnection> GetOrCreateConnectionAsync()
    {
        if (_connection is null || _connection.State != ConnectionState.Open)
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
        }

        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
            _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
