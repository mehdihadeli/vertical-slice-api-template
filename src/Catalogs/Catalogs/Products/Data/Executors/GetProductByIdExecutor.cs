using Catalogs.Products.ReadModel;

namespace Catalogs.Products.Data.Executors;

public delegate Task<ProductReadModel?> GetProductByIdExecutor(Guid id, CancellationToken cancellationToken);
