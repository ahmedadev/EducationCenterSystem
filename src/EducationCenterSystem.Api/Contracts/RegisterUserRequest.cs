namespace EducationCenterSystem.Api.Contracts;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber,
    Guid? RoleId);
