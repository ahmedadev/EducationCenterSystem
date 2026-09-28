namespace EducationCenterSystem.Domain.Entities;

public sealed class RolePermission
{
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }
    public int PermissionId { get; private set; }
    public Permission Permission { get; private set; }

    private RolePermission()
    {
        Role = default!;
        Permission = default!;
    }

    public RolePermission(Guid roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        Role = default!;
        Permission = default!;
    }
}
