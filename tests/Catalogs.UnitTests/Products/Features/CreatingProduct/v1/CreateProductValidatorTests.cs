using AutoBogus;
using Catalogs.Products.Features.CreatingProduct.v1;
using Catalogs.UnitTests.Common;
using FluentAssertions;
using Tests.Shared.XunitCategories;
using FluentValidation.TestHelper;

namespace Catalogs.UnitTests.Products.Features.CreatingProduct.v1;

public class CreateProductValidatorTests : CatalogsUnitTestBase
{
    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void must_success_with_valid_inputs()
    {
        // Arrange
        var command = new AutoFaker<CreateProduct>()
            .RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 10000))
            .Generate();
        var validator = new Validator();

        var result = validator.TestValidate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void must_fail_with_null_or_empty_name()
    {
        // Arrange
        var command = new AutoFaker<CreateProduct>().RuleFor(x => x.Name, _ => null!).Generate();
        var validator = new Validator();

        var result = validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
