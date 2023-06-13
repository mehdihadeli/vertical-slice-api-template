using Vertical.Slice.Template.Shared.Core.Ef;

namespace Vertical.Slice.Template.Shared.Data;

public class GenericRepository<T> : EfRepository<T, CatalogsDbContext>
    where T : class
{
    public GenericRepository(CatalogsDbContext dbContext)
        : base(dbContext) { }
}
