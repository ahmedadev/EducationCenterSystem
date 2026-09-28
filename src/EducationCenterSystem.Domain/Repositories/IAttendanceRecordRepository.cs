using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Domain.Repositories;

public interface IAttendanceRecordRepository
{
    Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttendanceRecord>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    void Add(AttendanceRecord record);
    void Update(AttendanceRecord record);
    void Remove(AttendanceRecord record);
}
