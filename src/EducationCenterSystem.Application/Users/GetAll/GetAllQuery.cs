using EducationCenterSystem.Application.Users.Common;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Users.GetAll;

public sealed record GetAllQuery : IRequest<ErrorOr<IReadOnlyList<UserResponse>>>;
