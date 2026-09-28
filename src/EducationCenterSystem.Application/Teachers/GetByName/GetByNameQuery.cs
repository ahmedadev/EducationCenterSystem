using EducationCenterSystem.Application.Teachers.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Teachers.GetByName;

public sealed record GetByNameQuery(string Name) : IRequest<ErrorOr<IReadOnlyList<TeacherResponse>>>;
