using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

public sealed class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CourseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .OrderBy(c => c.GradeLevel)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, string gradeLevel, string subject, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .AnyAsync(c => c.Name == name && c.GradeLevel == gradeLevel && c.Subject == subject, cancellationToken);
    }

    public void Add(Course course)
    {
        _dbContext.Courses.Add(course);
    }

    public void Update(Course course)
    {
        _dbContext.Courses.Update(course);
    }

    public void Remove(Course course)
    {
        _dbContext.Courses.Remove(course);
    }
}
