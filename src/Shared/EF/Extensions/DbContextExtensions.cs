using Microsoft.EntityFrameworkCore;

namespace Shared.EF.Extensions;

public static class DbContextExtensions
{
    public static async ValueTask InsertAndSaveAsync<T>(this DbContext dbContext, T entity, CancellationToken ct)
        where T : notnull
    {
        await dbContext.AddAsync(entity, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public static ValueTask<T?> FindAsync<T, TId>(this DbContext dbContext, TId id, CancellationToken ct)
        where T : class
        where TId : notnull => dbContext.FindAsync<T>(new object[] { id }, ct);

    public static IQueryable<TProject> ProjectEntity<T, TProject>(
        this DbContext dbContext,
        Func<IQueryable<T>, IQueryable<TProject>> projectionFunc,
        CancellationToken ct
    )
        where T : class
        where TProject : class
    {
        var projectedQuery = projectionFunc(dbContext.Set<T>());

        return projectedQuery.AsNoTracking();
    }
}
