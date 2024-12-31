using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Core.CQRS;
using Shared.Abstractions.Persistence.Ef;
using Shared.Cache;
using Shared.Core.Exceptions;
using Shared.Core.Extensions;
using Shared.EF.Extensions;
using Shared.Validation.Extensions;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Products.ReadModel;
using Vertical.Slice.Template.Shared.Data;

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

public class GetProductByIdValidator : AbstractValidator<GetProductById>
{
    public GetProductByIdValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}

internal class GetProductByIdHandler(DbExecutors.GetProductByIdExecutor getProductByIdExecutor)
    : IQueryHandler<GetProductById, GetProductByIdResult>
{
    public async ValueTask<GetProductByIdResult> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        request.NotBeNull();

        var productReadModel = await getProductByIdExecutor(request.Id, cancellationToken);

        if (productReadModel is null)
        {
            throw new NotFoundException($"product with id {request.Id} not found");
        }

        var productDto = productReadModel.ToProductDto();

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

            Task<ProductReadModel?> Query(Guid id, CancellationToken cancellationToken) =>
                context
                    .ProjectEntity<Product, ProductReadModel>(ProductMappings.ToProductsReadModel, cancellationToken)
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return Query;
        });
    }
}
