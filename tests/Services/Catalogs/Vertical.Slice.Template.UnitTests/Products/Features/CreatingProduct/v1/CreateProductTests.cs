using System.Diagnostics.CodeAnalysis;
using AutoBogus;
using FluentAssertions;
using FluentValidation.TestHelper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shared.Abstractions.Core.Messaging;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Products.Features.CreatingProduct.v1;

[Collection(UnitTestCollection.Name)]
[SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly")]
public class CreateProductTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_create_product_with_valid_inputs()
    {
        var createProductMock = new AutoFaker<CreateProduct>().Generate();
        await using var dbContext = CreateDbContext();

        // Arrange
        var handler = new CreateProductHandler(
            dbContext,
            Substitute.For<IMediator>(),
            Substitute.For<IExternalEventBus>(),
            NullLogger<CreateProductHandler>.Instance);

        // Act
        var createdCustomerResponse =
            await handler.Handle(createProductMock, CancellationToken.None);

        //Assert
        createdCustomerResponse.Should().NotBeNull();
        createdCustomerResponse.Id.Should().Be(createProductMock.Id);
        (await dbContext.Products.SingleAsync()).Id.Should().Be(createProductMock.Id);
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task Handle_ShouldPublishEventsWithCorrectData_WhenProductIsCreated()
    {
        // Arrange
        var createProductMock = new AutoFaker<CreateProduct>().Generate();

        var mediatorMock = Substitute.For<IMediator>();
        var eventBusMock = Substitute.For<IExternalEventBus>();
        await using var dbContext = CreateDbContext();

        mediatorMock.Publish(
                Arg.Is<ProductCreatedDomainEvent>(
                    e =>
                        e.Id == createProductMock.Id &&
                        e.Name == createProductMock.Name &&
                        e.CategoryId == createProductMock.CategoryId &&
                        e.Price == createProductMock.Price &&
                        e.Description == createProductMock.Description),
                Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask);


        eventBusMock.PublishAsync(
            Arg.Is<ProductCreatedIntegrationEventV1>(
                e =>
                    e.Id == createProductMock.Id &&
                    e.Name == createProductMock.Name &&
                    e.CategoryId == createProductMock.CategoryId &&
                    e.Price == createProductMock.Price &&
                    e.Description == createProductMock.Description),
            null,
            Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new CreateProductHandler(
            dbContext,
            mediatorMock,
            eventBusMock,
            NullLogger<CreateProductHandler>.Instance);

        // Act
        await handler.Handle(createProductMock, CancellationToken.None);

        // Assert
        await mediatorMock.Received(1)
            .Publish(
                Arg.Is<ProductCreatedDomainEvent>(
                    e =>
                        e.Id == createProductMock.Id &&
                        e.Name == createProductMock.Name &&
                        e.CategoryId == createProductMock.CategoryId &&
                        e.Price == createProductMock.Price &&
                        e.Description == createProductMock.Description),
                Arg.Any<CancellationToken>());

        await eventBusMock.Received(1)
            .PublishAsync(
                Arg.Is<ProductCreatedIntegrationEventV1>(
                    e =>
                        e.Id == createProductMock.Id &&
                        e.Name == createProductMock.Name &&
                        e.CategoryId == createProductMock.CategoryId &&
                        e.Price == createProductMock.Price &&
                        e.Description == createProductMock.Description),
                null,
                Arg.Any<CancellationToken>());
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public void validator_should_fail_for_invalid_inputs()
    {
        var validator = new CreateProductValidator();

        var invalidProducts = new (CreateProduct Command, string Property)[]
        {
            (new CreateProduct("", Guid.NewGuid(), 10), nameof(CreateProduct.Name)),
            (new CreateProduct("Pr", Guid.NewGuid(), 10), nameof(CreateProduct.Name)),
            (new CreateProduct("ValidName", Guid.Empty, 10), nameof(CreateProduct.CategoryId)),
            (new CreateProduct("ValidName", Guid.NewGuid(), -1), nameof(CreateProduct.Price)),
            (new CreateProduct("ValidName", Guid.NewGuid(), 0), nameof(CreateProduct.Price)),
        };

        foreach (var (command, property) in invalidProducts)
        {
            var validationResult = validator.TestValidate(command);
            validationResult.Errors.Should().Contain(error => error.PropertyName == property);
        }
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task should_persist_product_in_in_memory_database()
    {
        var createProductMock = new AutoFaker<CreateProduct>().Generate();
        await using var dbContext = CreateDbContext();

        var handler = new CreateProductHandler(
            dbContext,
            Substitute.For<IMediator>(),
            Substitute.For<IExternalEventBus>(),
            NullLogger<CreateProductHandler>.Instance);

        await handler.Handle(createProductMock, CancellationToken.None);

        var product = await dbContext.Products.SingleAsync();
        product.Id.Should().Be(createProductMock.Id);
        product.Name.Should().Be(createProductMock.Name);
        product.CategoryId.Should().Be(createProductMock.CategoryId);
        product.Price.Should().Be(createProductMock.Price);
        product.Description.Should().Be(createProductMock.Description);
    }

    private static CatalogsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CatalogsDbContext>()
            .UseInMemoryDatabase($"catalogs-unit-tests-{Guid.NewGuid()}")
            .Options;

        return new CatalogsDbContext(options);
    }
}
