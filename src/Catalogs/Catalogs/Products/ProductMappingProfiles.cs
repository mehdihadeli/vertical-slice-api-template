using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.CreatingProduct.v1;
using Catalogs.Products.ReadModel;

namespace Catalogs.Products;

public class ProductMappingProfiles : Profile
{
    public ProductMappingProfiles()
    {
        CreateMap<CreateProductRequest, CreateProduct>()
            .ConvertUsing(x => new CreateProduct(x.Name, x.CategoryId, x.Price, x.Description));
        CreateMap<Product, ProductReadModel>();
        CreateMap<CreateProduct, Product>();
        CreateMap<ProductReadModel, ProductLiteDto>();
        CreateMap<ProductReadModel, ProductDto>();
    }
}
