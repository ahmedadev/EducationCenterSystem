using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Primitives;

namespace EducationCenterSystem.Domain.Repositories;

public interface IParentRepository
{
    Task<Parent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Parent?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyCollection<Parent> Parents, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    void Add(Parent parent);
    void Update(Parent parent);
}
