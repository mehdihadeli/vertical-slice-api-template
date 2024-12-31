using AutoBogus;
using FluentAssertions;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared.Abstractions.Core.Messaging;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Products.Features.CreatingProduct.v1;

[Collection(UnitTestCollection.Name)]
public class CreateProductTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_create_product_with_valid_inputs()
    {
        var createProductMock = new AutoFaker<CreateProduct>().Generate();

        var executor = Substitute.For<DbExecuters.CreateAndSaveProductExecutor>();
        executor
            .Invoke(Arg.Is<Product>(x => x.Id == createProductMock.Id), Arg.Any<CancellationToken>())
            .Returns(new ValueTask());

        // Arrange
        var handler = new CreateProductHandler(
            executor,
            Substitute.For<IMediator>(),
            Substitute.For<IExternalEventBus>(),
            NullLogger<CreateProductHandler>.Instance
        );

        // Act
        var createdCustomerResponse = await handler.Handle(createProductMock, CancellationToken.None);

        //Assert
        await executor
            .Received(1)
            .Invoke(Arg.Is<Product>(x => x.Id == createProductMock.Id), Arg.Any<CancellationToken>());
        createdCustomerResponse.Should().NotBeNull();
        createdCustomerResponse.Id.Should().Be(createProductMock.Id);
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public void validator_should_fail_for_invalid_inputs()
    {
        var validator = new CreateProductValidator();

        var invalidProducts = new[]
        {
            new CreateProduct("", Guid.NewGuid(), 10), // Empty name
            new CreateProduct("Pr", Guid.NewGuid(), 10), // Name too short
            new CreateProduct("ValidName", Guid.Empty, 10), // Empty CategoryId
            new CreateProduct("ValidName", Guid.NewGuid(), -1), // Negative price
            new CreateProduct("ValidName", Guid.NewGuid(), 0), // Zero price
        };

        foreach (var invalidProduct in invalidProducts)
        {
            var validationResult = validator.Validate(invalidProduct);
            validationResult.IsValid.Should().BeFalse();
        }
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task should_handle_executor_exceptions_gracefully()
    {
        var createProductMock = new AutoFaker<CreateProduct>().Generate();

        var executor = Substitute.For<DbExecuters.CreateAndSaveProductExecutor>();
        executor.Invoke(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Throws(new Exception("Database error"));

        var handler = new CreateProductHandler(
            executor,
            Substitute.For<IMediator>(),
            Substitute.For<IExternalEventBus>(),
            NullLogger<CreateProductHandler>.Instance
        );

        Func<Task> act = async () => await handler.Handle(createProductMock, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
