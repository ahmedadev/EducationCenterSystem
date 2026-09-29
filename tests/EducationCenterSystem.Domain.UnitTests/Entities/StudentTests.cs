using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Events;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;

namespace EducationCenterSystem.Domain.UnitTests.Entities;

public sealed class StudentTests
{
    private readonly Email _validEmail = Email.Create("student@test.com").Value;
    private readonly PhoneNumber _validPhone = PhoneNumber.Create("01012345678").Value;
    private readonly PhoneNumber _validParentPhone = PhoneNumber.Create("01112345678").Value;

    [Fact]
    public void Register_ShouldReturnSuccessAndRaiseEvent_WhenAllParametersAreValid()
    {
        // Arrange
        var firstName = "أحمد";
        var lastName = "علاء";
        var dob = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var nationalId = "30801011234567";
        var gradeLevel = "الصف الأول الثانوي";
        var studentCode = "STU-0001";
        var schoolName = "مدرسة النيل";
        var gender = Gender.Male;
        var address = "القاهرة";
        var notes = "ملاحظات";

        // Act
        var result = Student.Register(
            firstName,
            "Second",
            "Third",
            lastName,
            _validEmail,
            _validPhone,
            dob,
            nationalId,
            _validParentPhone,
            gradeLevel,
            studentCode,
            schoolName,
            gender,
            address,
            notes);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.StudentCode.Should().Be(studentCode);
        result.Value.Status.Should().Be(StudentStatus.Active);
        result.Value.GetDomainEvents().Should().ContainSingle(e => e is StudentRegisteredEvent);
    }

    [Fact]
    public void Register_ShouldReturnValidationError_WhenFirstNameIsEmpty()
    {
        // Arrange
        var firstName = "   ";
        var lastName = "علاء";
        var dob = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = Student.Register(
            firstName,
            "Second",
            "Third",
            lastName,
            _validEmail,
            _validPhone,
            dob,
            "30801011234567",
            _validParentPhone,
            "الصف الأول الثانوي",
            "STU-0001",
            "مدرسة النيل",
            Gender.Male,
            "القاهرة",
            null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.FirstName");
    }

    [Fact]
    public void Register_ShouldReturnValidationError_WhenDateOfBirthIsInFuture()
    {
        // Arrange
        var futureDob = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Student.Register(
            "أحمد",
            "عادل",
            "محمد",
            "علاء",
            _validEmail,
            _validPhone,
            futureDob,
            "30801011234567",
            _validParentPhone,
            "الصف الأول الثانوي",
            "STU-0001",
            null,
            Gender.Male,
            null,
            null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.DateOfBirth");
    }

    [Fact]
    public void Register_ShouldReturnValidationError_WhenStudentCodeIsEmpty()
    {
        // Arrange
        var studentCode = "";

        // Act
        var result = Student.Register(
            "أحمد",
            "عادل",
            "محمد",
            "علاء",
            _validEmail,
            _validPhone,
            new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567",
            _validParentPhone,
            "الصف الأول الثانوي",
            studentCode,
            null,
            Gender.Male,
            null,
            null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.StudentCode");
    }

    [Fact]
    public void UpdateContactInfo_ShouldReturnValidationError_WhenEmailIsNull()
    {
        // Arrange
        var student = Student.Register(
            "أحمد", "عادل", "محمد", "علاء", _validEmail, _validPhone, new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567", _validParentPhone, "الصف الأول الثانوي", "STU-0001", null, Gender.Male, null, null).Value;

        // Act
        var result = student.UpdateContactInfo(null!, _validPhone, _validParentPhone, null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.Email");
    }

    [Fact]
    public void UpdateContactInfo_ShouldReturnValidationError_WhenPhoneIsNull()
    {
        // Arrange
        var student = Student.Register(
            "أحمد", "عادل", "محمد", "علاء", _validEmail, _validPhone, new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567", _validParentPhone, "الصف الأول الثانوي", "STU-0001", null, Gender.Male, null, null).Value;

        // Act
        var result = student.UpdateContactInfo(_validEmail, null!, _validParentPhone, null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Student.PhoneNumber");
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateStateSuccessfully_WhenDataIsValid()
    {
        // Arrange
        var student = Student.Register(
            "أحمد",
            "عادل",
            "محمد",
            "علاء",
            _validEmail,
            _validPhone,
            new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567",
            _validParentPhone,
            "الصف الأول الثانوي",
            "STU-0001",
            null,
            Gender.Male,
            null,
            null).Value;

        var newEmail = Email.Create("ahmed.new@test.com").Value;
        var newPhone = PhoneNumber.Create("01234567890").Value;

        // Act
        var updateResult = student.UpdateProfile(
            "محمود",
            "صلاح",
            "عمر",
            "السيد",
            newEmail,
            newPhone,
            new DateTime(2007, 5, 10, 0, 0, 0, DateTimeKind.Utc),
            "30705101234567",
            _validParentPhone,
            "الصف الثاني الثانوي",
            "STU-0001-UPDATED",
            "مدرسة اللغات",
            Gender.Male,
            "الجيزة",
            "تم تحديث البيانات");

        // Assert
        updateResult.IsError.Should().BeFalse();
        student.FirstName.Should().Be("محمود");
        student.LastName.Should().Be("السيد");
        student.Email.Value.Should().Be("ahmed.new@test.com");
        student.GradeLevel.Should().Be("الصف الثاني الثانوي");
        student.StudentCode.Should().Be("STU-0001-UPDATED");
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatusDirectly_WhenInvoked()
    {
        // Arrange
        var student = Student.Register(
            "أحمد",
            "عادل",
            "محمد",
            "علاء",
            _validEmail,
            _validPhone,
            new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "30801011234567",
            _validParentPhone,
            "الصف الأول الثانوي",
            "STU-0001",
            null,
            Gender.Male,
            null,
            null).Value;

        // Act
        student.ChangeStatus(StudentStatus.Suspended);

        // Assert
        student.Status.Should().Be(StudentStatus.Suspended);
    }
}
