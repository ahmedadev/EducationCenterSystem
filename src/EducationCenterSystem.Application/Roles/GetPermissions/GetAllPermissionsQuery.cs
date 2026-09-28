using EducationCenterSystem.Application.Roles.Common;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.GetPermissions;

public sealed record GetAllPermissionsQuery : IRequest<ErrorOr<IReadOnlyList<PermissionResponse>>>;
