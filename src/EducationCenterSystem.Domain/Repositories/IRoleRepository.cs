using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Domain.Repositories;

public interface IRoleRepository
{
    void Add(Role role);
    void Update(Role role);
    void Remove(Role role);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsRoleNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}
