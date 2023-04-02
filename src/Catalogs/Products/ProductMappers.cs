using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.ReadModel;

namespace Catalogs.Products;

public class ProductMappers : Profile
{
	public ProductMappers()
	{
		CreateMap<CreateProductRequest, CreateProduct>();
		CreateMap<Product, ProductReadModel>();
		CreateMap<ProductReadModel, ProductLiteDto>();
		CreateMap<ProductReadModel, ProductDto>();
	}
}