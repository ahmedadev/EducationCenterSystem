using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<ErrorOr<Success>>;
