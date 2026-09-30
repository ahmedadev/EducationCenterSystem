using EducationCenterSystem.Presentation.WinForms.Models;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface ITeacherApiService
{
    Task<List<TeacherModel>> GetAllTeachersAsync(CancellationToken cancellationToken = default);
    Task<PagedResultModel<TeacherModel>?> GetPagedTeachersAsync(int page, int pageSize, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null, CancellationToken cancellationToken = default);
    Task<TeacherModel?> GetTeacherByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CreateTeacherAsync(TeacherModel teacher, CancellationToken cancellationToken = default);
    Task<bool> UpdateTeacherAsync(Guid id, TeacherModel teacher, CancellationToken cancellationToken = default);
    Task<bool> DeleteTeacherAsync(Guid id, CancellationToken cancellationToken = default);
}
