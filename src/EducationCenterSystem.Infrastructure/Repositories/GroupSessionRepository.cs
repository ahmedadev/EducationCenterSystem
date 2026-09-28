using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

public sealed class GroupSessionRepository : IGroupSessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GroupSessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GroupSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupSessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<GroupSession?> GetByIdWithAttendanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupSessions
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<GroupSession>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupSessions
            .Where(s => s.EducationalGroupId == groupId)
            .OrderByDescending(s => s.SessionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForDateAsync(Guid groupId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupSessions
            .AnyAsync(s => s.EducationalGroupId == groupId && s.SessionDate.Date == date.Date, cancellationToken);
    }

    public void Add(GroupSession session)
    {
        _dbContext.GroupSessions.Add(session);
    }

    public void Update(GroupSession session)
    {
        _dbContext.GroupSessions.Update(session);
    }

    public void Remove(GroupSession session)
    {
        _dbContext.GroupSessions.Remove(session);
    }
}
