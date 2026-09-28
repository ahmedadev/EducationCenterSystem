using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;

namespace EducationCenterSystem.Domain.UnitTests.ValueObjects;

public sealed class ValueObjectTests
{
    [Theory]
    [InlineData("valid@example.com")]
    [InlineData("student123@education.eg")]
    [InlineData("first.last@domain.co.uk")]
    public void Email_Create_ShouldReturnSuccess_WhenFormatIsValid(string email)
    {
        // Act
        var result = Email.Create(email);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Email_Create_ShouldReturnValidationError_WhenEmpty(string? email)
    {
        // Act
        var result = Email.Create(email!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.Empty");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("username@.com")]
    public void Email_Create_ShouldReturnValidationError_WhenFormatIsInvalid(string email)
    {
        // Act
        var result = Email.Create(email);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.Format");
    }

    [Theory]
    [InlineData("01012345678")]
    [InlineData("01234567890")]
    [InlineData("+201012345678")]
    public void PhoneNumber_Create_ShouldReturnSuccess_WhenValid(string phone)
    {
        // Act
        var result = PhoneNumber.Create(phone);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(phone);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("0101")]
    [InlineData("01234567890123456")] // > 15 chars
    public void PhoneNumber_Create_ShouldReturnValidationError_WhenLengthInvalid(string phone)
    {
        // Act
        var result = PhoneNumber.Create(phone);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.Length");
    }
}
