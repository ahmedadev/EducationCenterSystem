namespace EducationCenterSystem.Application.Roles.Common;

public sealed record PermissionResponse(
    int Id,
    string Name,
    string Description,
    string Module);
