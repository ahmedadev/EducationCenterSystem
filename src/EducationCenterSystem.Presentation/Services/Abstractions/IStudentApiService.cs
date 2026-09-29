using EducationCenterSystem.Presentation.WinForms.Models;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IStudentApiService
{
    Task<PagedResultModel<StudentModel>?> GetPagedStudentsAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<StudentModel?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CreateStudentAsync(StudentModel student, CancellationToken cancellationToken = default);
    Task<bool> UpdateStudentAsync(Guid id, StudentModel student, CancellationToken cancellationToken = default);
    Task<bool> DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default);
}
