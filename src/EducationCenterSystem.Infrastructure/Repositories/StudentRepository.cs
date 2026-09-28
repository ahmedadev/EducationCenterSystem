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

    public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Students.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var allStudents = await query.ToListAsync(cancellationToken);
            var filtered = allStudents.Where(s => 
                FuzzyMatchContains($"{s.FirstName} {s.LastName}", searchTerm) || 
                FuzzyMatchContains(s.StudentCode, searchTerm) ||
                FuzzyMatchContains(s.NationalId, searchTerm) ||
                FuzzyMatchContains(s.PhoneNumber.Value, searchTerm)
            ).ToList();
            
            int skip = (page - 1) * pageSize;
            if (skip < 0) skip = 0;
            
            var items = filtered.OrderBy(s => s.StudentCode).Skip(skip).Take(pageSize).ToList();
            return (items, filtered.Count);
        }
        else
        {
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
    }

    private bool FuzzyMatchContains(string? source, string search, int maxDistance = 1)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(search)) return false;
        
        source = source.ToLowerInvariant();
        search = search.ToLowerInvariant();
        
        if (source.Contains(search)) return true;

        var sourceWords = source.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var searchWords = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var sWord in searchWords)
        {
            foreach (var srcWord in sourceWords)
            {
                if (srcWord.Contains(sWord) || LevenshteinDistance(srcWord, sWord) <= maxDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int[] v0 = new int[t.Length + 1];
        int[] v1 = new int[t.Length + 1];

        for (int i = 0; i < v0.Length; i++) v0[i] = i;

        for (int i = 0; i < s.Length; i++)
        {
            v1[0] = i + 1;
            for (int j = 0; j < t.Length; j++)
            {
                int cost = (s[i] == t[j]) ? 0 : 1;
                v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
            }
            for (int j = 0; j < v0.Length; j++) v0[j] = v1[j];
        }
        return v1[t.Length];
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
