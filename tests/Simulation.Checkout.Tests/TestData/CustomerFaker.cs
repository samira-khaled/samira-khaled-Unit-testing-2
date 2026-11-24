using Bogus;

namespace Simulation.Checkout.Tests.TestData;

internal sealed record TestCustomer(string Name, string Email);

internal static class CustomerFaker
{
    public const int Seed = 2026;

    public static TestCustomer Create()
    {
        var faker = new Faker("en") { Random = new Randomizer(Seed) };

        return new TestCustomer(
            faker.Name.FullName(),
            faker.Internet.Email());
    }
}