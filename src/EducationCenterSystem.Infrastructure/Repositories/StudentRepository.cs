using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

internal sealed class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StudentRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(Student student)
    {
        _dbContext.Students.Add(student);
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Student?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students.FirstOrDefaultAsync(s => s.NationalId == nationalId, cancellationToken);
    }

    public async Task<Student?> GetByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Students
            .AsNoTracking()
            .OrderBy(s => s.StudentCode)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Students.AsNoTracking();
        int totalCount = await query.CountAsync(cancellationToken);

        int skip = (page - 1) * pageSize;
        if (skip < 0) skip = 0;

        var items = await query
            .OrderBy(s => s.StudentCode)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Student>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync(cancellationToken);
        }

        var normalizedQuery = name.Trim();
        var terms = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var query = _dbContext.Students.AsNoTracking();

        if (terms.Length == 1)
        {
            var singleTerm = $"%{terms[0]}%";
            query = query.Where(s => EF.Functions.ILike(s.FirstName, singleTerm)
                                  || EF.Functions.ILike(s.LastName, singleTerm));
        }
        else
        {
            var firstTerm = $"%{terms[0]}%";
            var lastTerm = $"%{terms[^1]}%";
            var fullTerm = $"%{normalizedQuery}%";

            query = query.Where(s => (EF.Functions.ILike(s.FirstName, firstTerm) && EF.Functions.ILike(s.LastName, lastTerm))
                                  || EF.Functions.ILike(s.FirstName, fullTerm)
                                  || EF.Functions.ILike(s.LastName, fullTerm));
        }

        return await query
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Domain.ValueObjects.Email email, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Students.AnyAsync(s => s.Email.Value == email.Value, cancellationToken);
    }

    public void Update(Student student)
    {
        _dbContext.Students.Update(student);
    }

    public void Remove(Student student)
    {
        _dbContext.Students.Remove(student);
    }
}
