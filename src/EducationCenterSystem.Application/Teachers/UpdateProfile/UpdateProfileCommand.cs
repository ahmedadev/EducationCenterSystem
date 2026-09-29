using EducationCenterSystem.Domain.Enums;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid TeacherId,
    string FirstName,
    string SecondName,
    string ThirdName,
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
    string? Notes) : IRequest<ErrorOr<Success>>;
