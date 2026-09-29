namespace EducationCenterSystem.Presentation.WinForms.Models.DTOs;

public record EducationalGroupDto(Guid Id, Guid CourseId, string CourseName, Guid TeacherId, string TeacherName, string Name, int MaxCapacity, decimal MonthlyFee, string ScheduleDescription, int Status);
