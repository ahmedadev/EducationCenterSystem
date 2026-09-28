using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.ValueObjects;

namespace EducationCenterSystem.Domain.Repositories;

public interface ITeacherRepository
{
    void Add(Teacher teacher);
    Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Teacher?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    Task<Teacher?> GetByTeacherCodeAsync(string teacherCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Teacher> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Teacher>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
    void Update(Teacher teacher);
    void Remove(Teacher teacher);
}
