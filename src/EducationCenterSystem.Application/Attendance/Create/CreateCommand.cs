using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.Create;

public sealed record CreateCommand(
    Guid EducationalGroupId,
    DateTime SessionDate,
    string? Notes) : IRequest<ErrorOr<Guid>>;
