using FluentAssertions;
using NetArchTest.Rules;

namespace EducationCenterSystem.Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "EducationCenterSystem.Domain";
    private const string ApplicationNamespace = "EducationCenterSystem.Application";
    private const string InfrastructureNamespace = "EducationCenterSystem.Infrastructure";
    private const string PresentationNamespace = "EducationCenterSystem.Presentation";
    private const string ApiNamespace = "EducationCenterSystem.Api";

    [Fact]
    public void Domain_ShouldNot_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Domain.Entities.Student).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PresentationNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNot_HaveDependencyOnInfrastructureOrPresentationOrApi()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        var forbiddenProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_HaveNameEndingWithHandler()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(MediatR.IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
