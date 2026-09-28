using EducationCenterSystem.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Attendance.GetBySessionId;

public sealed class GetBySessionIdQueryHandler : IRequestHandler<GetBySessionIdQuery, ErrorOr<IReadOnlyList<AttendanceRecordResponse>>>
{
    private readonly IAttendanceRecordRepository _attendanceRepository;

    public GetBySessionIdQueryHandler(IAttendanceRecordRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<AttendanceRecordResponse>>> Handle(GetBySessionIdQuery request, CancellationToken cancellationToken)
    {
        var records = await _attendanceRepository.GetBySessionIdAsync(request.SessionId, cancellationToken);

        return records.Select(r => new AttendanceRecordResponse(
            r.Id,
            r.GroupSessionId,
            r.StudentId,
            r.Student != null ? $"{r.Student.FirstName} {r.Student.LastName}" : "Unknown",
            r.Status,
            r.Notes)).ToList().AsReadOnly();
    }
}
