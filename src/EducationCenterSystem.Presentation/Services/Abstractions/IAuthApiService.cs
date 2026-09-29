using EducationCenterSystem.Presentation.WinForms.Models.DTOs.Auth;

namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IAuthApiService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
