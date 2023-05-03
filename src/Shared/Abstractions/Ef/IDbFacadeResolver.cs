using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shared.Abstractions.Ef;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
