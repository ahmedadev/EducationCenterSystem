using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.GetBySessionId;

public sealed record GetBySessionIdQuery(Guid SessionId) : IRequest<ErrorOr<IReadOnlyList<AttendanceRecordResponse>>>;
