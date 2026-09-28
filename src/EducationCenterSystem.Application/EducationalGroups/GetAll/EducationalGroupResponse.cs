using EducationCenterSystem.Domain.Enums;

namespace EducationCenterSystem.Application.EducationalGroups.GetAll;

public sealed record EducationalGroupResponse(
    Guid Id,
    Guid CourseId,
    string CourseName,
    Guid TeacherId,
    string TeacherName,
    string Name,
    int MaxCapacity,
    decimal MonthlyFee,
    string ScheduleDescription,
    GroupStatus Status);
