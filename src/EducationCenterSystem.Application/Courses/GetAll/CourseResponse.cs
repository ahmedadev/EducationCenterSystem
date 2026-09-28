namespace EducationCenterSystem.Application.Courses.GetAll;

public sealed record CourseResponse(
    Guid Id,
    string Name,
    string GradeLevel,
    string Subject,
    string? Description,
    bool IsActive);
