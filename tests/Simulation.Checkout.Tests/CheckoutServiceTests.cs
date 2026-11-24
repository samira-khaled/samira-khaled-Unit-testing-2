using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Domain.Exceptions;
using Xunit;

namespace Simulation.Checkout.Tests;

public class CheckoutServiceTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly Mock<IInventoryService> _inventory = new();
    private readonly Mock<INotificationService> _notifications = new();
    private readonly CheckoutService _sut;

    public CheckoutServiceTests()
    {
        _inventory
            .Setup(x => x.IsAvailableAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _sut = new CheckoutService(
            _repository.Object, _inventory.Object, _notifications.Object);
    }

    private static Order CreateOrder(params OrderItem[] items) => new()
    {
        CustomerName = "Samira",
        CustomerEmail = "samira@test.com",
        Items = items
    };

    private static OrderItem CreateItem(
        decimal price = 100m,
        int quantity = 1,
        string name = "Lab") => new()
        {
            ProductId = Guid.NewGuid(),
            ProductName = name,
            UnitPrice = price,
            Quantity = quantity
        };

    // ---------- Task 1: Business rules ----------

    [Fact]
    public async Task CheckoutAsync_WhenOrderHasNoItems_ShouldThrowEmptyOrderException()
    {
        var order = CreateOrder();

        Func<Task> act = () => _sut.CheckoutAsync(order);

        await act.Should().ThrowAsync<EmptyOrderException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CheckoutAsync_WhenQuantityIsZeroOrNegative_ShouldThrowInvalidQuantityExceptionWithProductName(int quantity)
    {
        var order = CreateOrder(CreateItem(quantity: quantity, name: "Lab"));

        Func<Task> act = () => _sut.CheckoutAsync(order);

        await act.Should().ThrowAsync<InvalidQuantityException>()
            .WithMessage("*Lab*");
    }

    [Fact]
    public async Task CheckoutAsync_WhenPriceIsNegative_ShouldThrowInvalidPriceException()
    {
        var order = CreateOrder(CreateItem(price: -0.01m));

        Func<Task> act = () => _sut.CheckoutAsync(order);

        await act.Should().ThrowAsync<InvalidPriceException>();
    }

    [Fact]
    public async Task CheckoutAsync_WhenPriceIsZero_ShouldSucceed()
    {
        var order = CreateOrder(CreateItem(price: 0m));

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Total.Should().Be(0m);
    }

    public static TheoryData<decimal, decimal> DiscountCases => new()
    {
        { 999.99m, 0m },
        { 1000m, 100m },
        { 1500m, 100m }
    };

    [Theory]
    [MemberData(nameof(DiscountCases))]
    public async Task CheckoutAsync_WhenSubtotalVaries_ShouldApplyExpectedDiscount(decimal subtotal, decimal expectedDiscount)
    {
        var order = CreateOrder(CreateItem(price: subtotal, quantity: 1));

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Discount.Should().Be(expectedDiscount);
    }

    [Fact]
    public async Task CheckoutAsync_WhenExampleA_ShouldReturnCorrectSubtotalDiscountAndTotal()
    {
        var order = CreateOrder(
            CreateItem(price: 150m, quantity: 2, name: "Product A"),
            CreateItem(price: 400m, quantity: 1, name: "Product B"));

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Subtotal.Should().Be(700m);
        receipt.Discount.Should().Be(0m);
        receipt.Total.Should().Be(700m);
    }

    [Fact]
    public async Task CheckoutAsync_WhenExampleC_ShouldReturnCorrectSubtotalDiscountAndTotal()
    {
        var order = CreateOrder(
            CreateItem(price: 500m, quantity: 3, name: "Product C"));

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Subtotal.Should().Be(1500m);
        receipt.Discount.Should().Be(100m);
        receipt.Total.Should().Be(1400m);
    }
}