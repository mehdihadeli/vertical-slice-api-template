using System.Data.Common;

namespace Vertical.Slice.Template.Shared.Abstractions.Ef;

public interface IConnectionFactory : IDisposable
{
    Task<DbConnection> GetOrCreateConnectionAsync();
}
