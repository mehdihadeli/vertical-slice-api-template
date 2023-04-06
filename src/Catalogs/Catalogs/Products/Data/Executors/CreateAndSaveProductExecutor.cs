namespace Catalogs.Products.Data.Executors;

public delegate ValueTask CreateAndSaveProductExecutor(Product product, CancellationToken cancellationToken);
