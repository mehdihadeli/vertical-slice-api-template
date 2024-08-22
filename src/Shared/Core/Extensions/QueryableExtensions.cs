using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Shared.Abstractions.Core.Paging;
using Shared.Core.Paging;
using Sieve.Models;
using Sieve.Services;

namespace Shared.Core.Extensions;

// we should not operation related to Ef or Mongo here and we should design as general with IQueryable to work with any providers
public static class QueryableExtensions
{
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        CancellationToken cancellationToken
    )
        where TEntity : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        // https://github.com/Biarity/Sieve/issues/34#issuecomment-403817573
        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false); // Only

        var items = await result.ToAsyncEnumerable().ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        IConfigurationProvider configurationProvider,
        ISieveProcessor sieveProcessor,
        CancellationToken cancellationToken
    )
        where TEntity : class
        where TResult : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        // https://github.com/Biarity/Sieve/issues/34#issuecomment-403817573
        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false); // Only applies pagination

        var items = await result
            .ProjectTo<TResult>(configurationProvider)
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<TEntity, TResult> map,
        CancellationToken cancellationToken
    )
        where TEntity : class
        where TResult : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        // https://github.com/Biarity/Sieve/issues/34#issuecomment-403817573
        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false); // Only applies pagination

        var items = await result
            .Select(x => map(x))
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        IConfigurationProvider configuration,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TResult : class
    {
        IQueryable<TEntity> query = collection;
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return await query.ApplyPagingAsync<TEntity, TResult>(
            pageRequest,
            configuration,
            sieveProcessor,
            cancellationToken
        );
    }

    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
    {
        IQueryable<TEntity> query = collection;
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return await query.ApplyPagingAsync(pageRequest, sieveProcessor, cancellationToken);
    }
}
