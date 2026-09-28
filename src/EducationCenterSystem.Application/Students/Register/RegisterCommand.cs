using EducationCenterSystem.Domain.Enums;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Students.Register;

public sealed record RegisterCommand(
    string FirstName,
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
    string? Notes) : IRequest<ErrorOr<Guid>>;
