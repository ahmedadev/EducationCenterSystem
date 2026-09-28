namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface ITokenProvider
{
    string? Token { get; }
    string? CurrentUserName { get; }
    string? CurrentUserEmail { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool HasPermission(string permission);
    bool IsInRole(string role);
    void SetAuthentication(string token, string name, string email, IEnumerable<string> roles, IEnumerable<string> permissions);
    void Clear();
}
