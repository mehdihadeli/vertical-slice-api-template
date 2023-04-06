using AutoMapper;
using Catalogs.Products.Data.Executors;
using Catalogs.Products.Dtos;
using Catalogs.Products.ReadModel;
using FluentValidation;
using MediatR;
using Shared.Core.Exceptions;
using Shared.Core.Extensions;
using Shared.Core.Wrappers;
using Sieve.Services;

namespace Catalogs.Products.Features.GettingProductsByPage;

public record GetProductByPage(int PageSize, int PageNumber, string? Filters = null, string? SortOrder = null)
    : PageQuery<GetProductsResult>(PageSize, PageNumber, Filters, SortOrder);

internal class Validator : AbstractValidator<GetProductByPage>
{
    public Validator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

internal class GetProductByPageHandler : IRequestHandler<GetProductByPage, GetProductsResult>
{
    private readonly GetProductsExecutor _getProductsExecutor;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IValidator<GetProductByPage> _validator;
    private readonly IMapper _mapper;

    public GetProductByPageHandler(
        GetProductsExecutor getProductsExecutor,
        ISieveProcessor sieveProcessor,
        IValidator<GetProductByPage> validator,
        IMapper mapper
    )
    {
        _getProductsExecutor = getProductsExecutor;
        _sieveProcessor = sieveProcessor;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<GetProductsResult> Handle(GetProductByPage request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new BadRequestException(string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));
        }

        var query = _getProductsExecutor(request, cancellationToken);

        var pageList = await query.ApplyPaging<ProductReadModel, ProductDto>(
            request,
            _mapper.ConfigurationProvider,
            _sieveProcessor
        );

        return new GetProductsResult(pageList);
    }
}

public record GetProductsResult(IPageList<ProductDto> Products);
