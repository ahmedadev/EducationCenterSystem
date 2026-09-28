using EducationCenterSystem.Domain.Enums;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.Register;

public sealed record RegisterCommand(
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
    string? Notes) : IRequest<ErrorOr<Guid>>;
