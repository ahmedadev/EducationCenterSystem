using EducationCenterSystem.Presentation.WinForms.Models;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IRoleApiService
{
    Task<List<RoleModel>?> GetAllRolesAsync();
}
