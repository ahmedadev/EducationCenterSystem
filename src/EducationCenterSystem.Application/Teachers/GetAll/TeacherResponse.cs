using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Application.Teachers.GetAll;

public sealed record TeacherResponse(
    Guid Id,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    DateTime RegisteredOnUtc,
    string? NationalId,
    string TeacherCode,
    string Subject,
    string? Qualification,
    Gender Gender,
    string? Address,
    TeacherStatus Status,
    string? Notes);
