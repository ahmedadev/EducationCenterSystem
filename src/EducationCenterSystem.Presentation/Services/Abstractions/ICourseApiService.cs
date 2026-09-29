using EducationCenterSystem.Presentation.WinForms.Models.DTOs;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface ICourseApiService
{
    Task<List<CourseDto>?> GetAllCoursesAsync();
    Task<bool> CreateCourseAsync(object payload);
}
