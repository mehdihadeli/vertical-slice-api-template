using Catalogs.Products.ReadModel;
using Shared.Core.Wrappers;

namespace Catalogs.Products.Data.Executors;

public delegate IQueryable<ProductReadModel> GetProductsExecutor(
    IPageRequest request,
    CancellationToken cancellationToken
);
