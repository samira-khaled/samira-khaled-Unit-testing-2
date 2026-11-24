
using Bogus;
using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Xunit;

namespace Simulation.Checkout.Tests.TestData
{
    public class CheckoutServiceBogusTests
    {
        [Fact]
        public async Task CheckoutAsync_WhenCustomerIsGeneratedByBogus_ShouldReturnSameCustomerData()
        {
            // Arrange

            var orderFaker = new Faker<Order>()
                .UseSeed(2026)
                .RuleFor(x => x.CustomerName, f => f.Name.FullName())
                .RuleFor(x => x.CustomerEmail, f => f.Internet.Email())
                .RuleFor(x => x.Items, f => new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = f.Random.Guid(),
                        ProductName = f.Commerce.ProductName(),
                        UnitPrice = 100m,
                        Quantity = 1
                    }
                });

            var order = orderFaker.Generate();

            var inventory = new Mock<IInventoryService>();

            inventory.Setup(x => x.IsAvailableAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var repository = new Mock<IOrderRepository>();
            var notifications = new Mock<INotificationService>();

            var service = new CheckoutService(
                repository.Object,
                inventory.Object,
                notifications.Object);

            // Act

            var receipt = await service.CheckoutAsync(order);

            // Assert

            receipt.Should().NotBeNull();
            receipt.CustomerName.Should().Be(order.CustomerName);
            receipt.CustomerEmail.Should().Be(order.CustomerEmail);
        }
    }
}
