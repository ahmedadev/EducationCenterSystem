using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Application.Students.GetAll;

public sealed record StudentResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    DateTime RegisteredOnUtc,
    string? NationalId,
    string ParentPhoneNumber,
    string GradeLevel,
    string StudentCode,
    string? SchoolName,
    Gender Gender,
    string? Address,
    StudentStatus Status,
    string? Notes);
