namespace EducationCenterSystem.Presentation.WinForms.Models;

public sealed class RoleModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public List<string> Permissions { get; set; } = new();

    public int PermissionsCount => Permissions.Count;
    public bool IsCustomRole => !IsSystemRole;
    public string RoleTypeBadge => IsSystemRole ? "دور نظامي أساسي" : "دور مخصص";
    public string PermissionsSummary => PermissionsCount > 0 ? $"{PermissionsCount} صلاحية مفعلة" : "لا توجد صلاحيات";
}
