using EducationCenterSystem.Domain.Primitives;
using MediatR;

namespace EducationCenterSystem.Application.Common.Events;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
