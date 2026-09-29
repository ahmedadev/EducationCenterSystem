using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid Id,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    string? Email,
    string PhoneNumber,
    string? NationalId,
    string? Job,
    string? Address,
    string? Notes) : IRequest<ErrorOr<Success>>;
