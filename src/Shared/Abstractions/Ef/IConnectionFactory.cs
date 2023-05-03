using System.Data.Common;

namespace Shared.Abstractions.Ef;

public interface IConnectionFactory : IDisposable
{
    Task<DbConnection> GetOrCreateConnectionAsync();
}
