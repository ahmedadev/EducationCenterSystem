using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.EducationalGroups.Create;

public sealed record CreateCommand(
    Guid CourseId,
    Guid TeacherId,
    string Name,
    int MaxCapacity,
    decimal MonthlyFee,
    string ScheduleDescription) : IRequest<ErrorOr<Guid>>;
