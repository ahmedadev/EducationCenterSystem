using EducationCenterSystem.Application.Teachers.Register;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EducationCenterSystem.Application.UnitTests.Teachers;

public sealed class RegisterTeacherCommandHandlerTests
{
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterTeacherCommandHandlerTests()
    {
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RegisterCommandHandler(
            _teacherRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterTeacherCommandHandler_ShouldReturnSuccess_WhenTeacherDataIsValid()
    {
        // Arrange
        var command = new RegisterCommand(
            FirstName: "Mohamed",
            LastName: "Hassan",
            Email: "mohamed.hassan@example.com",
            PhoneNumber: "01098765432",
            DateOfBirth: new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            NationalId: "28503151234567",
            TeacherCode: "TCH-0001",
            Subject: "Mathematics",
            Qualification: "Faculty of Science",
            Gender: Gender.Male,
            Address: "Giza",
            Notes: "Senior Math Instructor");

        _teacherRepositoryMock
            .Setup(repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _teacherRepositoryMock
            .Setup(repo => repo.GetByNationalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        _teacherRepositoryMock
            .Setup(repo => repo.GetByTeacherCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        _teacherRepositoryMock.Verify(repo => repo.Add(It.IsAny<Teacher>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterTeacherCommandHandler_ShouldReturnConflict_WhenEmailAlreadyInUse()
    {
        // Arrange
        var command = new RegisterCommand(
            FirstName: "Mohamed",
            LastName: "Hassan",
            Email: "duplicate@example.com",
            PhoneNumber: "01098765432",
            DateOfBirth: new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            NationalId: "28503151234567",
            TeacherCode: "TCH-0002",
            Subject: "Physics",
            Qualification: "Faculty of Education",
            Gender: Gender.Male,
            Address: "Alexandria",
            Notes: null);

        _teacherRepositoryMock
            .Setup(repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.Email");

        _teacherRepositoryMock.Verify(repo => repo.Add(It.IsAny<Teacher>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterTeacherCommandHandler_ShouldReturnConflict_WhenTeacherCodeAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand(
            FirstName: "Tarek",
            LastName: "Mahmoud",
            Email: "tarek@example.com",
            PhoneNumber: "01234567890",
            DateOfBirth: new DateTime(1990, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            NationalId: null,
            TeacherCode: "TCH-DUPLICATE",
            Subject: "Chemistry",
            Qualification: "Master Degree",
            Gender: Gender.Male,
            Address: null,
            Notes: null);

        var existingTeacher = Teacher.Register(
            "Existing",
            "Teacher",
            Email.Create("exist@example.com").Value,
            PhoneNumber.Create("01011112222").Value,
            new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            null,
            "TCH-DUPLICATE",
            "Chemistry",
            "Bachelor",
            Gender.Male,
            null,
            null).Value;

        _teacherRepositoryMock
            .Setup(repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _teacherRepositoryMock
            .Setup(repo => repo.GetByTeacherCodeAsync("TCH-DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeacher);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.TeacherCode");

        _teacherRepositoryMock.Verify(repo => repo.Add(It.IsAny<Teacher>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
