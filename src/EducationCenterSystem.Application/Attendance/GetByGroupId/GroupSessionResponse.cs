namespace EducationCenterSystem.Application.Attendance.GetByGroupId;

public sealed record GroupSessionResponse(
    Guid Id,
    Guid EducationalGroupId,
    DateTime SessionDate,
    string? Notes,
    int TotalAttendees);
