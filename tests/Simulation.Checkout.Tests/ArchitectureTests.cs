using FluentAssertions;
using NetArchTest.Rules;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Infrastructure;
using System.Reflection;

namespace Simulation.Checkout.Tests;

public class ArchitectureTests
{
        private static readonly Assembly DomainAssembly = typeof(Order).Assembly;
        private static readonly Assembly ApplicationAssembly = typeof(CheckoutService).Assembly;
        private static readonly Assembly InfrastructureAssembly = typeof(InMemoryOrderRepository).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOnApplicationOrInfrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Simulation.Checkout.Application", "Simulation.Checkout.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Simulation.Checkout.Infrastructure")
            .GetResult();
        result.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        var result = Types.InAssemblies(new[] { DomainAssembly , ApplicationAssembly , InfrastructureAssembly })
        .That()
        .AreInterfaces()
        .Should()
        .HaveNameStartingWith("I")
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void OrderRepositoryImplementations_ShouldEndWithRepository()
    {
        var result = Types.InAssemblies(new[] { ApplicationAssembly, InfrastructureAssembly })
            .That()
            .ImplementInterface(typeof(IOrderRepository))
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}