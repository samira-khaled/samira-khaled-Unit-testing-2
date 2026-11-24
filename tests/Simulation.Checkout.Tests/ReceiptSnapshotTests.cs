using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using VerifyXunit;
using Xunit;

namespace Simulation.Checkout.Tests;


public class ReceiptSnapshotTests
{
    [Fact]
    public async Task CheckoutAsync_WhenOrderIsExampleC_ShouldMatchApprovedReceipt()
    {
        // Arrange
        var inventory = new Mock<IInventoryService>();
        inventory
            .Setup(x => x.IsAvailableAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new CheckoutService(
            new Mock<IOrderRepository>().Object,
            inventory.Object,
            new Mock<INotificationService>().Object);

        var order = new Order
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CustomerName = "Samira",
            CustomerEmail = "samira@test.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ProductName = "Product C",
                    UnitPrice = 500m,
                    Quantity = 3
                }
            }
        };

        // Act
        var receipt = await sut.CheckoutAsync(order);

        // Assert
        await Verifier.Verify(receipt)
            .IgnoreMember(nameof(CheckoutReceipt.CreatedAt));
    }
}