using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

public sealed class EducationalGroupRepository : IEducationalGroupRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EducationalGroupRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EducationalGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalGroups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<EducationalGroup?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalGroups
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EducationalGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalGroups
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EducationalGroup>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalGroups
            .Where(g => g.CourseId == courseId)
            .Include(g => g.Teacher)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EducationalGroups
            .AnyAsync(g => g.Name == name, cancellationToken);
    }

    public void Add(EducationalGroup group)
    {
        _dbContext.EducationalGroups.Add(group);
    }

    public void Update(EducationalGroup group)
    {
        _dbContext.EducationalGroups.Update(group);
    }

    public void Remove(EducationalGroup group)
    {
        _dbContext.EducationalGroups.Remove(group);
    }

    public void AddEnrollment(StudentGroup studentGroup)
    {
        _dbContext.StudentGroups.Add(studentGroup);
    }

    public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentGroups
            .AnyAsync(sg => sg.StudentId == studentId && sg.EducationalGroupId == groupId, cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetStudentsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentGroups
            .Where(sg => sg.EducationalGroupId == groupId && sg.Status == Domain.Enums.EnrollmentStatus.Active)
            .Include(sg => sg.Student)
            .Select(sg => sg.Student)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .ToListAsync(cancellationToken);
    }
}
