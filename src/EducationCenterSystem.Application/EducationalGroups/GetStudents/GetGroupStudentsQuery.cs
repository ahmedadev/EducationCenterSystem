using EducationCenterSystem.Application.Students.GetAll;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.GetStudents;

public sealed record GetGroupStudentsQuery(Guid GroupId) : IRequest<ErrorOr<IEnumerable<StudentResponse>>>;
