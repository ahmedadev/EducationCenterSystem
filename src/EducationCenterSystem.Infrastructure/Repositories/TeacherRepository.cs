using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

internal sealed class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TeacherRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(Teacher teacher)
    {
        _dbContext.Teachers.Add(teacher);
    }

    public async Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Teacher?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teachers.FirstOrDefaultAsync(t => t.NationalId == nationalId, cancellationToken);
    }

    public async Task<Teacher?> GetByTeacherCodeAsync(string teacherCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teachers.FirstOrDefaultAsync(t => t.TeacherCode == teacherCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teachers
            .AsNoTracking()
            .OrderBy(t => t.TeacherCode)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Teacher> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Teachers.AsNoTracking();
        int totalCount = await query.CountAsync(cancellationToken);

        int skip = (page - 1) * pageSize;
        if (skip < 0) skip = 0;

        var items = await query
            .OrderBy(t => t.TeacherCode)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Teacher>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync(cancellationToken);
        }

        var normalizedQuery = name.Trim();
        var terms = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var query = _dbContext.Teachers.AsNoTracking();

        if (terms.Length == 1)
        {
            var singleTerm = $"%{terms[0]}%";
            query = query.Where(t => EF.Functions.ILike(t.FirstName, singleTerm)
                                  || EF.Functions.ILike(t.LastName, singleTerm));
        }
        else
        {
            var firstTerm = $"%{terms[0]}%";
            var lastTerm = $"%{terms[^1]}%";
            var fullTerm = $"%{normalizedQuery}%";

            query = query.Where(t => (EF.Functions.ILike(t.FirstName, firstTerm) && EF.Functions.ILike(t.LastName, lastTerm))
                                  || EF.Functions.ILike(t.FirstName, fullTerm)
                                  || EF.Functions.ILike(t.LastName, fullTerm));
        }

        return await query
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Teachers.AnyAsync(t => t.Email.Value == email.Value, cancellationToken);
    }

    public void Update(Teacher teacher)
    {
        _dbContext.Teachers.Update(teacher);
    }

    public void Remove(Teacher teacher)
    {
        _dbContext.Teachers.Remove(teacher);
    }
}
