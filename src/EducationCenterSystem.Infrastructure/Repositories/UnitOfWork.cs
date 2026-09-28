using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EducationCenterSystem.Infrastructure.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IDomainEventDispatcher domainEventDispatcher,
        ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _domainEventDispatcher = domainEventDispatcher;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await _domainEventDispatcher.DispatchEventsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch domain events after saving database changes.");
            throw;
        }

        return result;
    }
}
