using EducationCenterSystem.Application.Common.Events;
using EducationCenterSystem.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EducationCenterSystem.Application.Students.Events;

internal sealed class StudentRegisteredDomainEventHandler : INotificationHandler<DomainEventNotification<StudentRegisteredEvent>>
{
    private readonly ILogger<StudentRegisteredDomainEventHandler> _logger;

    public StudentRegisteredDomainEventHandler(ILogger<StudentRegisteredDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<StudentRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event handled: Student registered successfully with ID: {StudentId}", notification.DomainEvent.StudentId);
        return Task.CompletedTask;
    }
}
