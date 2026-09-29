namespace EducationCenterSystem.Presentation.WinForms.Models.DTOs;

public record CourseDto(Guid Id, string Name, string GradeLevel, string Subject, string Description, bool IsActive);
