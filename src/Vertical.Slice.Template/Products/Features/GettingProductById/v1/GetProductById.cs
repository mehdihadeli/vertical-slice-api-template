using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Products.ReadModel;
using Vertical.Slice.Template.Shared.Abstractions.Core.CQRS;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.Shared.Cache;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Core.Extensions;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.Shared.EF.Extensions;
using Vertical.Slice.Template.Shared.Validation.Extensions;

namespace Vertical.Slice.Template.Products.Features.GettingProductById.v1;

public record GetProductById(Guid Id) : CacheQuery<GetProductById, GetProductByIdResult>
{
    /// <summary>
    /// GetProductById query with validation.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static GetProductById Of(Guid id)
    {
        return new GetProductByIdValidator().HandleValidation(new GetProductById(id));
    }

    public override string CacheKey(GetProductById request)
    {
        return $"{base.CacheKey(request)}_{request.Id}";
    }
}

internal class GetProductByIdValidator : AbstractValidator<GetProductById>
{
    public GetProductByIdValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}

internal class GetProductByIdHandler : IQueryHandler<GetProductById, GetProductByIdResult>
{
    private readonly DbExecutors.GetProductByIdExecutor _getProductByIdExecutor;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(DbExecutors.GetProductByIdExecutor getProductByIdExecutor, IMapper mapper)
    {
        _getProductByIdExecutor = getProductByIdExecutor;
        _mapper = mapper;
    }

    public async Task<GetProductByIdResult> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var productReadModel = await _getProductByIdExecutor(request.Id, cancellationToken);

        if (productReadModel is null)
        {
            throw new NotFoundException($"product with id {request.Id} not found");
        }

        var productDto = _mapper.Map<ProductDto>(productReadModel);

        return new GetProductByIdResult(productDto);
    }
}

public record GetProductByIdResult(ProductDto Product);

internal class DbExecutors : IDbExecutors
{
    public delegate Task<ProductReadModel?> GetProductByIdExecutor(Guid id, CancellationToken cancellationToken);

    public void Register(IServiceCollection services)
    {
        services.AddTransient<GetProductByIdExecutor>(sp =>
        {
            var context = sp.GetRequiredService<CatalogsDbContext>();
            var mapper = sp.GetRequiredService<IMapper>();

            Task<ProductReadModel?> Query(Guid id, CancellationToken cancellationToken) =>
                context
                    .ProjectEntity<Product, ProductReadModel>(mapper.ConfigurationProvider, cancellationToken)
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return Query;
        });
    }
}
