using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.Register;

public sealed record RegisterCommand(
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    string? Email,
    string PhoneNumber,
    string? NationalId,
    string? Job,
    string? Address,
    string? Notes) : IRequest<ErrorOr<Guid>>;
