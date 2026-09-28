using EducationCenterSystem.Application.Roles.Common;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Roles.GetAll;

public sealed record GetAllQuery : IRequest<ErrorOr<IReadOnlyList<RoleResponse>>>;
