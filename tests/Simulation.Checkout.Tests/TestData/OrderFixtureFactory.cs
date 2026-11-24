using AutoFixture;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

internal static class OrderFixtureFactory
{
    public static Order CreateValidOrder()
    {
        var fixture = new Fixture();

        fixture.Customize<OrderItem>(composer => composer
            .With(x => x.Quantity, () => Random.Shared.Next(1, 10))
            .With(x => x.UnitPrice, () => Random.Shared.Next(0, 500)));

        fixture.Customize<Order>(composer => composer
            .With(o => o.Items, () => fixture.CreateMany<OrderItem>(3).ToList()));

        return fixture.Create<Order>();
    }
}