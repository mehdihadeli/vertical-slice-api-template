using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Vertical.Slice.Template.Shared.Abstractions.Ef;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
