using EducationCenterSystem.Application.Teachers.Delete;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EducationCenterSystem.Application.UnitTests.Teachers;

public sealed class DeleteCommandHandlerTests
{
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteCommandHandler _handler;

    public DeleteCommandHandlerTests()
    {
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteCommandHandler(_teacherRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTeacherExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = Teacher.Register(
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

        var command = new DeleteCommand(teacherId);

        _teacherRepositoryMock.Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        _teacherRepositoryMock.Verify(x => x.Remove(teacher), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenTeacherDoesNotExist()
    {
        // Arrange
        var command = new DeleteCommand(Guid.NewGuid());

        _teacherRepositoryMock.Setup(x => x.GetByIdAsync(command.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.NotFound");
        _teacherRepositoryMock.Verify(x => x.Remove(It.IsAny<Teacher>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
