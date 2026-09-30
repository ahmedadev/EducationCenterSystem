using EducationCenterSystem.Domain.Enums;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.BatchRecord;

public sealed record BatchAttendanceRecordDto(
    Guid StudentId,
    AttendanceStatus Status,
    string? Notes,
    DateTime? CheckInTime,
    DateTime? CheckOutTime);

public sealed record BatchRecordCommand(
    Guid GroupId,
    DateTime SessionDate,
    List<BatchAttendanceRecordDto> Records) : IRequest<ErrorOr<Success>>;
