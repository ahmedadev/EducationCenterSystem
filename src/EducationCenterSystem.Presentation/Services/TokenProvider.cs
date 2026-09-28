using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.Services;

public sealed class TokenProvider : ITokenProvider
{
    private readonly HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _roles = new();

    public string? Token { get; private set; }
    public string? CurrentUserName { get; private set; }
    public string? CurrentUserEmail { get; private set; }
    public IReadOnlyList<string> Roles => _roles.AsReadOnly();
    public IReadOnlyList<string> Permissions => _permissions.ToList().AsReadOnly();

    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        // Super admins with "Admin" role or wildcard have full access
        if (IsInRole("Admin"))
        {
            return true;
        }

        return _permissions.Contains(permission);
    }

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return _roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }

    public void SetAuthentication(
        string token,
        string name,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        Token = token;
        CurrentUserName = name;
        CurrentUserEmail = email;

        _roles.Clear();
        _roles.AddRange(roles);

        _permissions.Clear();
        foreach (var permission in permissions)
        {
            _permissions.Add(permission);
        }
    }

    public void Clear()
    {
        Token = null;
        CurrentUserName = null;
        CurrentUserEmail = null;
        _roles.Clear();
        _permissions.Clear();
    }
}
