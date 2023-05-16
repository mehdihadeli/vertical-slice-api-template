using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Vertical.Slice.Template.Shared.EF.Extensions;

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
        IConfigurationProvider configurationProvider,
        CancellationToken ct
    )
        where T : class
        where TProject : class => dbContext.Set<T>().AsNoTracking().ProjectTo<TProject>(configurationProvider, ct);
}
