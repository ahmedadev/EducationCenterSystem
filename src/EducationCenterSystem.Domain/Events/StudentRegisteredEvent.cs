using EducationCenterSystem.Domain.Primitives;

namespace EducationCenterSystem.Domain.Events;

public sealed record StudentRegisteredEvent(Guid StudentId) : IDomainEvent;
