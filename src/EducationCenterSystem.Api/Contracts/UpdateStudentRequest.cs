using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Api.Contracts;

public sealed record UpdateStudentRequest(
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string? NationalId,
    string ParentPhoneNumber,
    string GradeLevel,
    string StudentCode,
    string? SchoolName,
    Gender Gender,
    string? Address,
    string? Notes);
