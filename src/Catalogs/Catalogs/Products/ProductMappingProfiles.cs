using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.Features.CreatingProduct.v1;
using Catalogs.Products.Features.GettingProductById.v1;
using Catalogs.Products.Features.GettingProductsByPage.v1;
using Catalogs.Products.Models;
using Catalogs.Products.ReadModel;

namespace Catalogs.Products;

public class ProductMappingProfiles : Profile
{
    public ProductMappingProfiles()
    {
        CreateMap<Product, ProductReadModel>();
        CreateMap<ProductReadModel, ProductDto>();

        CreateMap<CreateProductRequest, CreateProduct>();
        CreateMap<CreateProduct, Product>();
        CreateMap<CreateProductResult, CreateProductResponse>();

        CreateMap<GetProductsByPageRequestParameters, GetProductsByPage>();
        CreateMap<GetProductsByPageResult, GetGetProductsByPageResponse>();

        CreateMap<GetProductByIdRequestParameters, GetProductById>();
        CreateMap<GetProductByIdResult, GetProductByIdResponse>();
    }
}
