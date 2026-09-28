using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetAll;

public sealed record GetAllQuery() : IRequest<ErrorOr<IReadOnlyList<TeacherResponse>>>;
