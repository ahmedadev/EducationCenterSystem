using EducationCenterSystem.Application.Common.Events;
using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Domain.Primitives;
using MediatR;

namespace EducationCenterSystem.Infrastructure.Services;

internal sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(ApplicationDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = _dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.GetDomainEvents().Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        if (aggregateRoots.Count == 0)
        {
            return;
        }

        var domainEvents = aggregateRoots
            .SelectMany(aggregate => aggregate.GetDomainEvents())
            .ToList();

        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification is INotification mediatrNotification)
            {
                await _publisher.Publish(mediatrNotification, cancellationToken);
            }
        }
    }
}
