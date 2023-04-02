using AutoMapper;
using Catalogs.Products.Data.Executors;
using Catalogs.Products.Dtos;
using FluentValidation;
using MediatR;
using Shared.Core.Exceptions;

namespace Catalogs.Products.Features.GettingProductById;

public record GetProductById(Guid Id) : IRequest<GetProductByIdResult>;

internal class Validator : AbstractValidator<GetProductById>
{
	public Validator()
	{
		RuleFor(r => r.Id).NotEmpty();
	}
}

internal class GetProductByIdHandler : IRequestHandler<GetProductById, GetProductByIdResult>
{
	private readonly GetProductByIdExecutor _getProductByIdExecutor;
	private readonly IValidator<GetProductById> _validator;
	private readonly IMapper _mapper;

	public GetProductByIdHandler(
		GetProductByIdExecutor getProductByIdExecutor,
		IValidator<GetProductById> validator,
		IMapper mapper)
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

		var productDto = _mapper.Map<ProductLiteDto>(productReadModel);

		return new GetProductByIdResult(productDto);
	}
}

internal record GetProductByIdResult(ProductLiteDto Product);