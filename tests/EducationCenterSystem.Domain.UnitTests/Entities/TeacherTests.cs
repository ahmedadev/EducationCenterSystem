using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Events;
using EducationCenterSystem.Domain.ValueObjects;
using FluentAssertions;

namespace EducationCenterSystem.Domain.UnitTests.Entities;

public sealed class TeacherTests
{
    private readonly Email _validEmail = Email.Create("teacher@test.com").Value;
    private readonly PhoneNumber _validPhone = PhoneNumber.Create("01098765432").Value;

    [Fact]
    public void Register_ShouldReturnSuccessAndRaiseEvent_WhenAllParametersAreValid()
    {
        // Arrange
        var firstName = "طارق";
        var lastName = "محمود";
        var teacherCode = "TCH-0100";
        var subject = "اللغة الإنجليزية";

        // Act
        var result = Teacher.Register(
            firstName,
            "الثاني",
            "الثالث",
            lastName,
            _validEmail,
            _validPhone,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            teacherCode,
            subject,
            "ماجستير لغويات",
            Gender.Male,
            "مدينة نصر، القاهرة",
            "معلم أول أ");

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.TeacherCode.Should().Be(teacherCode);
        result.Value.Subject.Should().Be(subject);
        result.Value.Status.Should().Be(TeacherStatus.Active);
        result.Value.GetDomainEvents().Should().ContainSingle(e => e is TeacherRegisteredEvent);
    }

    [Fact]
    public void Register_ShouldReturnValidationError_WhenSubjectIsEmpty()
    {
        // Arrange
        var subject = "";

        // Act
        var result = Teacher.Register(
            "طارق",
            "الثاني",
            "الثالث",
            "محمود",
            _validEmail,
            _validPhone,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            "TCH-0100",
            subject,
            null,
            Gender.Male,
            null,
            null);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Teacher.Subject");
    }

    [Fact]
    public void UpdateContactInfo_ShouldUpdateContactFields_WhenValid()
    {
        // Arrange
        var teacher = Teacher.Register(
            "طارق",
            "الثاني",
            "الثالث",
            "محمود",
            _validEmail,
            _validPhone,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            "TCH-0100",
            "اللغة الإنجليزية",
            null,
            Gender.Male,
            "قديم",
            null).Value;

        var newEmail = Email.Create("new.teacher@test.com").Value;
        var newPhone = PhoneNumber.Create("01233445566").Value;

        // Act
        var result = teacher.UpdateContactInfo(newEmail, newPhone, "عنوان جديد");

        // Assert
        result.IsError.Should().BeFalse();
        teacher.Email.Value.Should().Be("new.teacher@test.com");
        teacher.PhoneNumber.Value.Should().Be("01233445566");
        teacher.Address.Should().Be("عنوان جديد");
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateProfileFields_WhenValidDataProvided()
    {
        // Arrange
        var teacher = Teacher.Register(
            "طارق",
            "الثاني",
            "الثالث",
            "محمود",
            _validEmail,
            _validPhone,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            "TCH-0100",
            "اللغة الإنجليزية",
            null,
            Gender.Male,
            "قديم",
            null).Value;

        var newEmail = Email.Create("new.email@test.com").Value;
        var newPhone = PhoneNumber.Create("01112223344").Value;

        // Act
        var result = teacher.UpdateProfile(
            "أحمد",
            "علي",
            "حسن",
            "مصطفى",
            newEmail,
            newPhone,
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "29001011234567",
            "TCH-0200",
            "الرياضيات",
            "بكالوريوس",
            Gender.Male,
            "القاهرة",
            "ملاحظة");

        // Assert
        result.IsError.Should().BeFalse();
        teacher.FirstName.Should().Be("أحمد");
        teacher.SecondName.Should().Be("علي");
        teacher.Email.Value.Should().Be("new.email@test.com");
        teacher.PhoneNumber.Value.Should().Be("01112223344");
        teacher.Subject.Should().Be("الرياضيات");
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus()
    {
        // Arrange
        var teacher = Teacher.Register(
            "طارق",
            "الثاني",
            "الثالث",
            "محمود",
            _validEmail,
            _validPhone,
            new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            "28503151234567",
            "TCH-0100",
            "اللغة الإنجليزية",
            null,
            Gender.Male,
            "قديم",
            null).Value;

        // Act
        teacher.ChangeStatus(TeacherStatus.Inactive);

        // Assert
        teacher.Status.Should().Be(TeacherStatus.Inactive);
    }
}
