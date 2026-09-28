using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Domain.Repositories;

public interface IStudentRepository
{
    void Add(Student student);
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Student?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    Task<Student?> GetByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(ValueObjects.Email email, CancellationToken cancellationToken = default);
    void Update(Student student);
    void Remove(Student student);
}
