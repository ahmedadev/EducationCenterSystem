using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.Events;
using EducationCenterSystem.Domain.Primitives;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class Student : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime RegisteredOnUtc { get; private set; }

    // New Properties
    public string? NationalId { get; private set; }
    public PhoneNumber ParentPhoneNumber { get; private set; }
    public string GradeLevel { get; private set; }
    public string StudentCode { get; private set; }
    public string? SchoolName { get; private set; }
    public Gender Gender { get; private set; }
    public string? Address { get; private set; }
    public StudentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<StudentGroup> _enrollments = new();
    public IReadOnlyCollection<StudentGroup> Enrollments => _enrollments.AsReadOnly();

    // Parameterless constructor for EF Core only
    private Student() : base()
    {
        FirstName = default!;
        LastName = default!;
        Email = default!;
        PhoneNumber = default!;
        ParentPhoneNumber = default!;
        GradeLevel = default!;
        StudentCode = default!;
    }

    private Student(
        Guid id,
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        DateTime registeredOnUtc,
        string? nationalId,
        PhoneNumber parentPhoneNumber,
        string gradeLevel,
        string studentCode,
        string? schoolName,
        Gender gender,
        string? address,
        StudentStatus status,
        string? notes) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        RegisteredOnUtc = registeredOnUtc;
        
        NationalId = nationalId;
        ParentPhoneNumber = parentPhoneNumber;
        GradeLevel = gradeLevel;
        StudentCode = studentCode;
        SchoolName = schoolName;
        Gender = gender;
        Address = address;
        Status = status;
        Notes = notes;
    }

    public static ErrorOr<Student> Register(
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        string? nationalId,
        PhoneNumber parentPhoneNumber,
        string gradeLevel,
        string studentCode,
        string? schoolName,
        Gender gender,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("Student.FirstName", "First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Student.LastName", "Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(gradeLevel))
            return Error.Validation("Student.GradeLevel", "Grade level cannot be empty.");

        if (string.IsNullOrWhiteSpace(studentCode))
            return Error.Validation("Student.StudentCode", "Student code cannot be empty.");

        var student = new Student(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phoneNumber,
            dateOfBirth,
            DateTime.UtcNow,
            nationalId,
            parentPhoneNumber,
            gradeLevel,
            studentCode,
            schoolName,
            gender,
            address,
            StudentStatus.Active,
            notes);

        student.RaiseDomainEvent(new StudentRegisteredEvent(student.Id));

        return student;
    }

    public ErrorOr<Success> UpdateContactInfo(Email email, PhoneNumber phoneNumber, PhoneNumber parentPhoneNumber, string? address)
    {
        if (email is null)
            return Error.Validation("Student.Email", "Email cannot be null.");
        if (phoneNumber is null)
            return Error.Validation("Student.PhoneNumber", "Phone number cannot be null.");
        if (parentPhoneNumber is null)
            return Error.Validation("Student.ParentPhoneNumber", "Parent phone number cannot be null.");

        Email = email;
        PhoneNumber = phoneNumber;
        ParentPhoneNumber = parentPhoneNumber;
        Address = address;

        return Result.Success;
    }

    public ErrorOr<Success> UpdateProfile(
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateTime dateOfBirth,
        string? nationalId,
        PhoneNumber parentPhoneNumber,
        string gradeLevel,
        string studentCode,
        string? schoolName,
        Gender gender,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("Student.FirstName", "First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation("Student.LastName", "Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(gradeLevel))
            return Error.Validation("Student.GradeLevel", "Grade level cannot be empty.");

        if (string.IsNullOrWhiteSpace(studentCode))
            return Error.Validation("Student.StudentCode", "Student code cannot be empty.");

        if (email is null)
            return Error.Validation("Student.Email", "Email cannot be null.");

        if (phoneNumber is null)
            return Error.Validation("Student.PhoneNumber", "Phone number cannot be null.");

        if (parentPhoneNumber is null)
            return Error.Validation("Student.ParentPhoneNumber", "Parent phone number cannot be null.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        NationalId = nationalId;
        ParentPhoneNumber = parentPhoneNumber;
        GradeLevel = gradeLevel;
        StudentCode = studentCode;
        SchoolName = schoolName;
        Gender = gender;
        Address = address;
        Notes = notes;

        return Result.Success;
    }

    public void ChangeStatus(StudentStatus newStatus)
    {
        Status = newStatus;
    }
}
