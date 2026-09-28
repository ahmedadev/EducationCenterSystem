using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Create;

public sealed record CreateCommand(
    string Name,
    string GradeLevel,
    string Subject,
    string? Description) : IRequest<ErrorOr<Guid>>;
