using AutoFixture;
using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Xunit;

namespace Simulation.Checkout.Tests;

public class CheckoutServiceAutoFixtureTests
{
    [Fact]
    public async Task CheckoutAsync_WhenOrderIsGeneratedByAutoFixture_ShouldSucceed()
    {
        // Arrange
        var fixture = new Fixture();

        fixture.Customize<OrderItem>(composer => composer
          .With(x => x.Quantity, 1)
          .With(x => x.UnitPrice, 100m));

        fixture.Customize<Order>(composer => composer
        .With(order => order.Items, fixture.CreateMany<OrderItem>(1).ToList()));

        var order = fixture.Create<Order>();
       
       
        var inventory = new Mock<IInventoryService>();

        inventory.Setup(x => x.IsAvailableAsync(
            It.IsAny<Guid>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var repository = new Mock<IOrderRepository>();
        var notifications = new Mock<INotificationService>();

        var service = new CheckoutService(repository.Object,inventory.Object,notifications.Object);
        // Act
        var receipt = await service.CheckoutAsync(order);

        // Assert

        order.Items.Should().OnlyContain(x =>x.Quantity > 0 && x.UnitPrice >= 0);
        receipt.Should().NotBeNull();
    }

}