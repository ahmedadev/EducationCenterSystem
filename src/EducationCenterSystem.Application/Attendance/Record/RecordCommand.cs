using EducationCenterSystem.Domain.Enums;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.Record;

public sealed record RecordCommand(
    Guid GroupSessionId,
    Guid StudentId,
    AttendanceStatus Status,
    string? Notes) : IRequest<ErrorOr<Guid>>;
