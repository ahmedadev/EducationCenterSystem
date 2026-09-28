using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber,
    Guid? RoleId) : IRequest<ErrorOr<Guid>>;
