using EducationCenterSystem.Presentation.WinForms.Models;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IUserApiService
{
    Task<List<UserModel>?> GetAllUsersAsync();
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
}
