using EducationCenterSystem.Application.Teachers.UpdateProfile;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EducationCenterSystem.Application.UnitTests.Teachers;

public sealed class UpdateProfileCommandHandlerTests
{
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateProfileCommandHandler _handler;
    private readonly Teacher _existingTeacher;

    public UpdateProfileCommandHandlerTests()
    {
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateProfileCommandHandler(_teacherRepositoryMock.Object, _unitOfWorkMock.Object);

        _existingTeacher = Teacher.Register(
            "طارق",
            "الثاني",
            "الثالث",
            "محمود",
            Email.Create("teacher@test.com").Value,
            PhoneNumber.Create("01098765432").Value,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            "TCH-0100",
            "اللغة الإنجليزية",
            null,
            Gender.Male,
            null,
            null).Value;
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllDataIsValid()
    {
        // Arrange
        var command = new UpdateProfileCommand(
            _existingTeacher.Id,
            "أحمد",
            "علي",
            "الثالث",
            "حسن",
            "newteacher@test.com",
            "01122334455",
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "29001011234567",
            "TCH-0200",
            "الرياضيات",
            "بكالوريوس",
            Gender.Male,
            "العنوان الجديد",
            "ملاحظات");

        _teacherRepositoryMock.Setup(x => x.GetByIdAsync(command.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingTeacher);

        _teacherRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _teacherRepositoryMock.Setup(x => x.GetByNationalIdAsync(command.NationalId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        _teacherRepositoryMock.Setup(x => x.GetByTeacherCodeAsync(command.TeacherCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        _existingTeacher.FirstName.Should().Be("أحمد");
        _existingTeacher.Email.Value.Should().Be("newteacher@test.com");
        _teacherRepositoryMock.Verify(x => x.Update(_existingTeacher), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenTeacherDoesNotExist()
    {
        // Arrange
        var command = new UpdateProfileCommand(
            Guid.NewGuid(), "أحمد", "علي", "الثالث", "حسن", "newteacher@test.com", "01122334455",
            DateTime.UtcNow, "29001011234567", "TCH-0200", "الرياضيات", null, Gender.Male, null, null);

        _teacherRepositoryMock.Setup(x => x.GetByIdAsync(command.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnConflictError_WhenEmailIsAlreadyInUseByAnotherTeacher()
    {
        // Arrange
        var command = new UpdateProfileCommand(
            _existingTeacher.Id, "أحمد", "علي", "الثالث", "حسن", "used@test.com", "01122334455",
            DateTime.UtcNow, null, "TCH-0200", "الرياضيات", null, Gender.Male, null, null);

        _teacherRepositoryMock.Setup(x => x.GetByIdAsync(command.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingTeacher);

        _teacherRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Indicates email is taken

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.Email");
    }
}
