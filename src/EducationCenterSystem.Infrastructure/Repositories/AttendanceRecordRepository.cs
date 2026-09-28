using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

public sealed class AttendanceRecordRepository : IAttendanceRecordRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AttendanceRecordRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AttendanceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AttendanceRecord>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .Include(a => a.Student)
            .Where(a => a.GroupSessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AttendanceRecord>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .Include(a => a.GroupSession)
            .ThenInclude(s => s!.EducationalGroup)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.GroupSession!.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public void Add(AttendanceRecord record)
    {
        _dbContext.AttendanceRecords.Add(record);
    }

    public void Update(AttendanceRecord record)
    {
        _dbContext.AttendanceRecords.Update(record);
    }

    public void Remove(AttendanceRecord record)
    {
        _dbContext.AttendanceRecords.Remove(record);
    }
}
