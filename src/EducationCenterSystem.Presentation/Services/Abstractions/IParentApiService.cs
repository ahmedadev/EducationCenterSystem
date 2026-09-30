using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IParentApiService
{
    Task<List<ParentDto>?> GetAllParentsAsync();
    Task<PagedResultModel<ParentDto>?> GetPagedParentsAsync(int page, int pageSize, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
    Task<bool> CreateParentAsync(ParentModel parent);
    Task<bool> UpdateParentAsync(ParentModel parent);
}
