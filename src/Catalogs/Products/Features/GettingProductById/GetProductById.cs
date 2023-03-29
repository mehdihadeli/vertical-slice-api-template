using AutoMapper;
using Carter.ModelBinding;
using Catalogs.Products.Dtos;
using Catalogs.Products.ReadModel;
using Catalogs.Shared.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
	private readonly CatalogsDbContext _catalogsDbContext;
	private readonly IValidator<GetProductById> _validator;
	private readonly IMapper _mapper;

	public GetProductByIdHandler(
		CatalogsDbContext catalogsDbContext,
		IValidator<GetProductById> validator,
		IMapper mapper)
	{
		_catalogsDbContext = catalogsDbContext;
		_validator = validator;
		_mapper = mapper;
	}

	public async Task<GetProductByIdResult> Handle(GetProductById request, CancellationToken cancellationToken)
	{
		var result = await _validator.ValidateAsync(request, cancellationToken);
		if (!result.IsValid)
		{
			throw new BadRequestException(string.Join(',', result.GetValidationProblems().SelectMany(x => x.Value)));
		}

		var productReadModel = await _catalogsDbContext.Products.Select(
									   x => new ProductReadModel
										    {
											    Id = x.Id,
											    Name = x.Name
										    })
								   .AsNoTracking()
								   .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

		if (productReadModel is null)
		{
			throw new NotFoundException($"product with id {request.Id} not found");
		}

		var productDto = _mapper.Map<ProductLiteDto>(productReadModel);

		return new GetProductByIdResult(productDto);
	}
}

internal record GetProductByIdResult(ProductLiteDto Product);