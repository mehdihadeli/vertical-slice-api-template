using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shared.Abstractions.Persistence.Ef;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
