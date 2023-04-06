using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Wrappers;
using Sieve.Models;
using Sieve.Services;

namespace Shared.Core.Extensions;

public static class QueryableExtensions
{
    public static async Task<IPageList<TResult>> ApplyPaging<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        IConfigurationProvider configurationProvider,
        ISieveProcessor sieveProcessor
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
        var total = await result.CountAsync();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false); // Only applies pagination

        var items = await result.ProjectTo<TResult>(configurationProvider).ToListAsync();

        return PageList<TResult>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }
}
