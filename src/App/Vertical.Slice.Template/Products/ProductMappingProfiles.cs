using AutoMapper;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Products.ReadModel;

namespace Vertical.Slice.Template.Products;

public class ProductMappingProfiles : Profile
{
    public ProductMappingProfiles()
    {
        CreateMap<Product, ProductReadModel>();
        CreateMap<ProductReadModel, ProductDto>();
        CreateMap<Catalogs.ApiClient.ProductDto, Product>();

        CreateMap<CreateProductRequest, CreateProduct>();
        CreateMap<CreateProduct, Product>();
        CreateMap<CreateProductResult, CreateProductResponse>();

        CreateMap<GetProductsByPageRequestParameters, GetProductsByPage>();
        CreateMap<GetProductsByPageResult, GetProductsByPageResponse>();

        CreateMap<GetProductByIdRequestParameters, GetProductById>();
        CreateMap<GetProductByIdResult, GetProductByIdResponse>();
    }
}
