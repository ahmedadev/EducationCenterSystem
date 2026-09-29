using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.AddStudent;

public sealed record AddStudentCommand(Guid GroupId, Guid StudentId) : IRequest<ErrorOr<Success>>;
