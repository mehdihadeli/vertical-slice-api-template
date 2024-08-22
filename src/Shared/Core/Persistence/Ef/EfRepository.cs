using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Persistence.Ef.Repository;

namespace Shared.Core.Persistence.Ef;

// inherit from Ardalis.Specification type
public class EfRepository<T, TContext> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class
    where TContext : DbContext
{
    public EfRepository(TContext dbContext)
        : base(dbContext) { }
}
