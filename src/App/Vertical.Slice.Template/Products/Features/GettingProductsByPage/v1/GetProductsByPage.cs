using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Abstractions.Core.Paging;
using Shared.Abstractions.Ef;
using Shared.Core.Extensions;
using Shared.Core.Paging;
using Shared.Validation.Extensions;
using Sieve.Services;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Products.ReadModel;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;

public record GetProductsByPage : PageQuery<GetProductsByPageResult>
{
    /// <summary>
    /// GetProductById query with validation.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    public static GetProductsByPage Of(PageRequest pageRequest)
    {
        var (pageNumber, pageSize, filters, sortOrder) = pageRequest;

        return new GetProductsByPageValidator().HandleValidation(
            new GetProductsByPage
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                SortOrder = sortOrder
            }
        );
    }
}

internal class GetProductsByPageValidator : AbstractValidator<GetProductsByPage>
{
    public GetProductsByPageValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

internal class GetProductByPageHandler : IRequestHandler<GetProductsByPage, GetProductsByPageResult>
{
    private readonly DbExecutors.GetProductsExecutor _getProductsExecutor;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public GetProductByPageHandler(
        DbExecutors.GetProductsExecutor getProductsExecutor,
        ISieveProcessor sieveProcessor,
        IMapper mapper
    )
    {
        _getProductsExecutor = getProductsExecutor;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
    }

    public async Task<GetProductsByPageResult> Handle(GetProductsByPage request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var query = _getProductsExecutor(cancellationToken);

        var pageList = await query.ApplyPagingAsync<Product, ProductReadModel>(
            request,
            _mapper.ConfigurationProvider,
            _sieveProcessor,
            cancellationToken
        );
        var result = pageList.MapTo<ProductDto>(_mapper);

        return new GetProductsByPageResult(result);
    }
}

public record GetProductsByPageResult(IPageList<ProductDto> Products);

internal class DbExecutors : IDbExecutors
{
    // public delegate ValueTask CreateAndSaveProductExecutor(Product product, CancellationToken cancellationToken);
    public delegate IQueryable<Product> GetProductsExecutor(CancellationToken cancellationToken);

    public void Register(IServiceCollection services)
    {
        services.AddTransient<GetProductsExecutor>(sp =>
        {
            var context = sp.GetRequiredService<CatalogsDbContext>();
            var mapper = sp.GetRequiredService<IMapper>();

            IQueryable<Product> Query(CancellationToken cancellationToken)
            {
                var collection = context.Products;

                return collection;
            }

            return Query;
        });
    }
}
