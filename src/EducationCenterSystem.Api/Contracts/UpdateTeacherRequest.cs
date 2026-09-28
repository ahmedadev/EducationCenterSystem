using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Api.Contracts;

public sealed record UpdateTeacherRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string? NationalId,
    string TeacherCode,
    string Subject,
    string? Qualification,
    Gender Gender,
    string? Address,
    string? Notes);
