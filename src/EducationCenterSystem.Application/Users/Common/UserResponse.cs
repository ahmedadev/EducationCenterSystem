namespace EducationCenterSystem.Application.Users.Common;

public sealed record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    bool IsActive,
    DateTime CreatedOnUtc,
    DateTime? LastLoginOnUtc,
    IReadOnlyList<string> Roles);
