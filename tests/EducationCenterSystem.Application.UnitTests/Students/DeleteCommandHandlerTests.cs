using EducationCenterSystem.Application.Students.Delete;
using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EducationCenterSystem.Application.UnitTests.Students;

public sealed class DeleteCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteCommandHandler _handler;

    public DeleteCommandHandlerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteCommandHandler(_studentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenStudentExists()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Register(
            "أحمد",
            "علاء",
            Email.Create("student@test.com").Value,
            PhoneNumber.Create("01012345678").Value,
            new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567",
            PhoneNumber.Create("01112345678").Value,
            "الصف الأول الثانوي",
            "STU-0001",
            "مدرسة النيل",
            Gender.Male,
            "القاهرة",
            null).Value;

        var command = new DeleteCommand(studentId);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        _studentRepositoryMock.Verify(x => x.Remove(student), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenStudentDoesNotExist()
    {
        // Arrange
        var command = new DeleteCommand(Guid.NewGuid());

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(command.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.NotFound");
        _studentRepositoryMock.Verify(x => x.Remove(It.IsAny<Student>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
