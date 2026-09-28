using EducationCenterSystem.Domain.Primitives;

namespace EducationCenterSystem.Domain.Events;

public sealed record TeacherRegisteredEvent(Guid TeacherId) : IDomainEvent;
