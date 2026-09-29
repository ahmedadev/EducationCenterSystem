using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Events;
using EducationCenterSystem.Domain.Primitives;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class Teacher : AggregateRoot
{
    public string FirstName { get; private set; }
    public string SecondName { get; private set; }
    public string ThirdName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime RegisteredOnUtc { get; private set; }
    public string? NationalId { get; private set; }
    public string TeacherCode { get; private set; }
    public string Subject { get; private set; }
    public string? Qualification { get; private set; }
    public Gender Gender { get; private set; }
    public string? Address { get; private set; }
    public TeacherStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<EducationalGroup> _groups = new();
    public IReadOnlyCollection<EducationalGroup> Groups => _groups.AsReadOnly();

    // Parameterless constructor for EF Core only
    private Teacher() : base()
    {
        FirstName = default!;
        SecondName = default!;
        ThirdName = default!;
        LastName = default!;
        Email = default!;
        PhoneNumber = default!;
        TeacherCode = default!;
        Subject = default!;
    }

    private Teacher(
        Guid id,
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        DateTime registeredOnUtc,
        string? nationalId,
        string teacherCode,
        string subject,
        string? qualification,
        Gender gender,
        string? address,
        TeacherStatus status,
        string? notes) : base(id)
    {
        FirstName = firstName;
        SecondName = secondName;
        ThirdName = thirdName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        RegisteredOnUtc = registeredOnUtc;
        NationalId = nationalId;
        TeacherCode = teacherCode;
        Subject = subject;
        Qualification = qualification;
        Gender = gender;
        Address = address;
        Status = status;
        Notes = notes;
    }

    public static ErrorOr<Teacher> Register(
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        string? nationalId,
        string teacherCode,
        string subject,
        string? qualification,
        Gender gender,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("Teacher.FirstName", "First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(secondName))
            return Error.Validation("Teacher.SecondName", "Second name cannot be empty.");

        if (string.IsNullOrWhiteSpace(thirdName))
            return Error.Validation("Teacher.ThirdName", "Third name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Teacher.LastName", "Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(teacherCode))
            return Error.Validation("Teacher.TeacherCode", "Teacher code cannot be empty.");

        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation("Teacher.Subject", "Subject cannot be empty.");

        if (email is null)
            return Error.Validation("Teacher.Email", "Email cannot be null.");

        if (phoneNumber is null)
            return Error.Validation("Teacher.PhoneNumber", "Phone number cannot be null.");

        var teacher = new Teacher(
            Guid.NewGuid(),
            firstName,
            secondName,
            thirdName,
            lastName,
            email,
            phoneNumber,
            dateOfBirth,
            DateTime.UtcNow,
            nationalId,
            teacherCode,
            subject,
            qualification,
            gender,
            address,
            TeacherStatus.Active,
            notes);

        teacher.RaiseDomainEvent(new TeacherRegisteredEvent(teacher.Id));

        return teacher;
    }

    public ErrorOr<Success> UpdateContactInfo(Email email, PhoneNumber phoneNumber, string? address)
    {
        if (email is null)
            return Error.Validation("Teacher.Email", "Email cannot be null.");

        if (phoneNumber is null)
            return Error.Validation("Teacher.PhoneNumber", "Phone number cannot be null.");

        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;

        return Result.Success;
    }

    public ErrorOr<Success> UpdateProfile(
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        string? nationalId,
        string teacherCode,
        string subject,
        string? qualification,
        Gender gender,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("Teacher.FirstName", "First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(secondName))
            return Error.Validation("Teacher.SecondName", "Second name cannot be empty.");

        if (string.IsNullOrWhiteSpace(thirdName))
            return Error.Validation("Teacher.ThirdName", "Third name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Teacher.LastName", "Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(teacherCode))
            return Error.Validation("Teacher.TeacherCode", "Teacher code cannot be empty.");

        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation("Teacher.Subject", "Subject cannot be empty.");

        if (email is null)
            return Error.Validation("Teacher.Email", "Email cannot be null.");

        if (phoneNumber is null)
            return Error.Validation("Teacher.PhoneNumber", "Phone number cannot be null.");

        FirstName = firstName;
        SecondName = secondName;
        ThirdName = thirdName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        NationalId = nationalId;
        TeacherCode = teacherCode;
        Subject = subject;
        Qualification = qualification;
        Gender = gender;
        Address = address;
        Notes = notes;

        return Result.Success;
    }

    public void ChangeStatus(TeacherStatus newStatus)
    {
        Status = newStatus;
    }
}
