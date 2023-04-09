using AutoBogus;
using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.CreatingProduct.v1;
using Catalogs.Products.ReadModel;
using Catalogs.UnitTests.Common;
using FluentAssertions;
using Tests.Shared.XunitCategories;

namespace Catalogs.UnitTests.Products;

public class ProductsMappingTests : IClassFixture<MappingFixture>
{
    private readonly IMapper _mapper;

    public ProductsMappingTests(MappingFixture fixture)
    {
        _mapper = fixture.Mapper;
    }

    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void must_success_with_valid_configuration()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void can_map_create_product_request_to_create_product()
    {
        var createProductRequest = AutoFaker.Generate<CreateProductRequest>();
        var res = _mapper.Map<CreateProduct>(createProductRequest);
        res.Name.Should().Be(createProductRequest.Name);
        res.Price.Should().Be(createProductRequest.Price);
        res.CategoryId.Should().Be(createProductRequest.CategoryId);
    }

    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void can_map_product_read_to_product_dto()
    {
        var productRead = AutoFaker.Generate<ProductReadModel>();
        var res = _mapper.Map<ProductDto>(productRead);
        res.Name.Should().Be(productRead.Name);
        res.Price.Should().Be(productRead.Price);
        res.CategoryId.Should().Be(productRead.CategoryId);
    }
}
