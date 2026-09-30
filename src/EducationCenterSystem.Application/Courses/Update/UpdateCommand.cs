using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Courses.Update;

public sealed record UpdateCommand(
    Guid Id,
    string Name,
    string GradeLevel,
    string Subject,
    string? Description) : IRequest<ErrorOr<Success>>;
