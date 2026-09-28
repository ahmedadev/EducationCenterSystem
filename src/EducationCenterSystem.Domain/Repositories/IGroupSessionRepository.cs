using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Domain.Repositories;

public interface IGroupSessionRepository
{
    Task<GroupSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GroupSession?> GetByIdWithAttendanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupSession>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForDateAsync(Guid groupId, DateTime date, CancellationToken cancellationToken = default);
    void Add(GroupSession session);
    void Update(GroupSession session);
    void Remove(GroupSession session);
}
