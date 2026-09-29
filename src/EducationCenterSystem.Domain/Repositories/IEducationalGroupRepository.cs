using EducationCenterSystem.Domain.Entities;

namespace EducationCenterSystem.Domain.Repositories;

public interface IEducationalGroupRepository
{
    Task<EducationalGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EducationalGroup?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EducationalGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EducationalGroup>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    void Add(EducationalGroup group);
    void Update(EducationalGroup group);
    void Remove(EducationalGroup group);
    
    // Enrollment methods could be here or in a separate repository
    void AddEnrollment(StudentGroup studentGroup);
    Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetStudentsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}
