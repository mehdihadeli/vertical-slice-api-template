using Riok.Mapperly.Abstractions;
using Vertical.Slice.Template.Products.Dtos.v1;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Products.ReadModel;

namespace Vertical.Slice.Template.Products;

// https://mapperly.riok.app/docs/configuration/static-mappers/
[Mapper]
internal static partial class ProductMappings
{
    internal static partial ProductReadModel ToProductReadModel(this Product product);

    internal static partial ProductDto ToProductDto(this ProductReadModel productReadModel);

    internal static partial Product ToProduct(this ProductDto productDto);

    internal static partial Product ToProduct(this Catalogs.ApiClient.ProductDto productDto);

    internal static partial CreateProduct ToCreateProduct(this CreateProductRequest createProductRequest);

    internal static partial Product ToProduct(this CreateProduct createProduct);

    internal static partial CreateProductResponse ToCreateProductResponse(this CreateProductResult createProductResult);

    internal static partial GetProductsByPage ToGetProductsByPage(
        this GetProductsByPageRequestParameters getProductsByPageRequestParameters
    );

    internal static partial GetProductsByPageResponse ToGetProductsByPageResponse(
        this GetProductsByPageResult getProductsByPageResult
    );

    internal static partial GetProductById ToGetProductById(
        this GetProductByIdRequestParameters getProductByIdRequestParameters
    );

    internal static partial GetProductByIdResponse ToGetProductByIdResponse(
        this GetProductByIdResult getProductByIdResult
    );

    internal static partial IList<Product> ToProducts(
        this ICollection<Catalogs.ApiClient.ProductDto> getProductByIdResult
    );

    internal static partial IQueryable<ProductReadModel> ToProductsReadModel(this IQueryable<Product> products);
}
