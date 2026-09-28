using EducationCenterSystem.Application.Students.Register;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EducationCenterSystem.Application.UnitTests.Students;

public sealed class RegisterStudentCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterStudentCommandHandlerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RegisterCommandHandler(
            _studentRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterStudentCommandHandler_ShouldReturnSuccess_WhenStudentDataIsValid()
    {
        // Arrange
        var command = new RegisterCommand(
            FirstName: "Ahmed",
            LastName: "Alaa",
            Email: "ahmed@example.com",
            PhoneNumber: "01012345678",
            DateOfBirth: new DateTime(2005, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            NationalId: "30505011234567",
            ParentPhoneNumber: "01112345678",
            GradeLevel: "الصف الثالث الثانوي",
            StudentCode: "STU-0001",
            SchoolName: "STEM School",
            Gender: Gender.Male,
            Address: "Cairo",
            Notes: "Excellent student");

        _studentRepositoryMock
            .Setup(repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        _studentRepositoryMock.Verify(
            repo => repo.Add(It.Is<Student>(s => s.FirstName == "Ahmed" && s.LastName == "Alaa")),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterStudentCommandHandler_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand(
            FirstName: "Ahmed",
            LastName: "Alaa",
            Email: "existing@example.com",
            PhoneNumber: "01012345678",
            DateOfBirth: new DateTime(2005, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            NationalId: "30505011234567",
            ParentPhoneNumber: "01112345678",
            GradeLevel: "الصف الثالث الثانوي",
            StudentCode: "STU-0001",
            SchoolName: "STEM School",
            Gender: Gender.Male,
            Address: "Cairo",
            Notes: null);

        _studentRepositoryMock
            .Setup(repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.Email");
        result.FirstError.Description.Should().Be("The email is already in use.");

        _studentRepositoryMock.Verify(
            repo => repo.Add(It.IsAny<Student>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
