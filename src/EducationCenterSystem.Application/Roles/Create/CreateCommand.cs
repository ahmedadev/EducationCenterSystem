using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.Create;

public sealed record CreateCommand(
    string Name,
    string Description,
    IReadOnlyList<int> PermissionIds) : IRequest<ErrorOr<Guid>>;
