using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.ReadModel;
using Catalogs.Shared.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Core;
using Shared.Core.Exceptions;
using Shared.EF.Extensions;

namespace Catalogs.Products.Features.GettingProductById.v1;

internal record GetProductById(Guid Id) : IRequest<GetProductByIdResult>;

internal class Validator : AbstractValidator<GetProductById>
{
    public Validator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}

internal class GetProductByIdHandler : IRequestHandler<GetProductById, GetProductByIdResult>
{
    private readonly DbExecutors.GetProductByIdExecutor _getProductByIdExecutor;
    private readonly IValidator<GetProductById> _validator;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(
        DbExecutors.GetProductByIdExecutor getProductByIdExecutor,
        IValidator<GetProductById> validator,
        IMapper mapper
    )
    {
        _getProductByIdExecutor = getProductByIdExecutor;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<GetProductByIdResult> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new BadRequestException(string.Join(',', result.Errors.Select(x => x.ErrorMessage)));
        }

        var productReadModel = await _getProductByIdExecutor(request.Id, cancellationToken);

        if (productReadModel is null)
        {
            throw new NotFoundException($"product with id {request.Id} not found");
        }

        var productDto = _mapper.Map<ProductDto>(productReadModel);

        return new GetProductByIdResult(productDto);
    }
}

internal record GetProductByIdResult(ProductDto Product);

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
