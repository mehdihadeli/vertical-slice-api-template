using System.Data.Common;

namespace Shared.Abstractions.Persistence.Ef;

public interface IConnectionFactory : IDisposable
{
    Task<DbConnection> GetOrCreateConnectionAsync();
}
