namespace EducationCenterSystem.Application.UnitTests.Students;

public sealed class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateProfileCommandHandler _handler;
    private readonly Student _existingStudent;

    public UpdateProfileCommandHandlerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateProfileCommandHandler(_studentRepositoryMock.Object, _unitOfWorkMock.Object);

        _existingStudent = Student.Register(
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
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllDataIsValid()
    {
        // Arrange
        var command = new UpdateProfileCommand(
            _existingStudent.Id,
            "محمود",
            "حسن",
            "newstudent@test.com",
            "01222334455",
            new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567",
            "01555667788",
            "الصف الثاني الثانوي",
            "STU-0002",
            "مدرسة المعارف",
            Gender.Male,
            "العنوان الجديد",
            "ملاحظات معدلة");

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(command.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingStudent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        _existingStudent.FirstName.Should().Be("محمود");
        _existingStudent.GradeLevel.Should().Be("الصف الثاني الثانوي");
        _studentRepositoryMock.Verify(x => x.Update(_existingStudent), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenStudentDoesNotExist()
    {
        // Arrange
        var command = new UpdateProfileCommand(
            Guid.NewGuid(), "محمود", "حسن", "newstudent@test.com", "01222334455",
            DateTime.UtcNow.AddYears(-15), "30801011234567", "01555667788", "الصف الثاني الثانوي", "STU-0002", null, Gender.Male, null, null);

        _studentRepositoryMock.Setup(x => x.GetByIdAsync(command.StudentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.NotFound");
    }
}
