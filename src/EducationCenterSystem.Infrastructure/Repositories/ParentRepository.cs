using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Infrastructure.Repositories;

internal sealed class ParentRepository : IParentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ParentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Parent parent)
    {
        _dbContext.Parents.Add(parent);
    }

    public async Task<Parent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parents
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Parent?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parents
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.NationalId == nationalId, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Parent> Parents, int TotalCount)> GetPagedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Parents
            .Include(p => p.Children)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm}%";
            query = query.Where(p => EF.Functions.ILike(p.FirstName, term) ||
                                     EF.Functions.ILike(p.SecondName, term) ||
                                     EF.Functions.ILike(p.ThirdName, term) ||
                                     EF.Functions.ILike(p.LastName, term) ||
                                     EF.Functions.ILike(p.PhoneNumber.Value, term) ||
                                     EF.Functions.ILike(p.NationalId!, term) ||
                                     p.Children.Any(c => EF.Functions.ILike(c.FirstName, term) ||
                                                         EF.Functions.ILike(c.SecondName, term) ||
                                                         EF.Functions.ILike(c.ThirdName, term) ||
                                                         EF.Functions.ILike(c.LastName, term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var parents = await query
            .OrderByDescending(p => p.RegisteredOnUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (parents, totalCount);
    }

    public void Update(Parent parent)
    {
        _dbContext.Parents.Update(parent);
    }
}
