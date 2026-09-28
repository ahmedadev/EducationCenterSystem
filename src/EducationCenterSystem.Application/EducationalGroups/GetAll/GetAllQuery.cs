using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.GetAll;

public sealed record GetAllQuery() : IRequest<ErrorOr<IReadOnlyList<EducationalGroupResponse>>>;
