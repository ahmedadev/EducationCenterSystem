using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Application.Attendance.GetBySessionId;

public sealed record AttendanceRecordResponse(
    Guid Id,
    Guid GroupSessionId,
    Guid StudentId,
    string StudentName,
    AttendanceStatus Status,
    string? Notes);
