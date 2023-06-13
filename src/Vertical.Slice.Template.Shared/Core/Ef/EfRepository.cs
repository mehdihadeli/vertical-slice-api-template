using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.Shared.Abstractions.Ef.Repository;

namespace Vertical.Slice.Template.Shared.Core.Ef;

// inherit from Ardalis.Specification type
public class EfRepository<T, TContext> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class
    where TContext : DbContext
{
    public EfRepository(TContext dbContext)
        : base(dbContext) { }
}
