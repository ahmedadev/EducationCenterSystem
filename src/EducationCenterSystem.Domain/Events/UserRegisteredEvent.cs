using EducationCenterSystem.Domain.Primitives;

namespace EducationCenterSystem.Domain.Events;

public sealed record UserRegisteredEvent(Guid UserId, string Email) : IDomainEvent;
