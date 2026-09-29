using EducationCenterSystem.Presentation.WinForms.Models.DTOs;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IEducationalGroupApiService
{
    Task<List<EducationalGroupDto>?> GetAllGroupsAsync();
    Task<List<EducationalGroupStudentDto>?> GetGroupStudentsAsync(Guid groupId);
    Task<bool> EnrollStudentAsync(Guid groupId, Guid studentId);
    Task<bool> CreateGroupAsync(object payload);
}
