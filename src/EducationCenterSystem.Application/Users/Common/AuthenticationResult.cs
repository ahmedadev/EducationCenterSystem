namespace EducationCenterSystem.Application.Users.Common;

public sealed record AuthenticationResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    string Token);
