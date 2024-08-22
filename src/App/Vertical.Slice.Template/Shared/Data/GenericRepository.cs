using Shared.Core.Persistence.Ef;

namespace Vertical.Slice.Template.Shared.Data;

public class GenericRepository<T>(CatalogsDbContext dbContext) : EfRepository<T, CatalogsDbContext>(dbContext)
    where T : class;
