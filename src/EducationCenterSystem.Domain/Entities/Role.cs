using EducationCenterSystem.Domain.Primitives;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class Role : AggregateRoot
{
    private readonly List<RolePermission> _permissions = new();

    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() : base()
    {
        Name = default!;
        Description = default!;
    }

    private Role(Guid id, string name, string description, bool isSystemRole) : base(id)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public static ErrorOr<Role> Create(string name, string description, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Role.NameEmpty", "اسم الدور لا يمكن أن يكون فارغاً.");
        }

        if (name.Trim().Length > 50)
        {
            return Error.Validation("Role.NameTooLong", "اسم الدور يجب ألا يتجاوز 50 حرفاً.");
        }

        return new Role(Guid.NewGuid(), name.Trim(), description?.Trim() ?? string.Empty, isSystemRole);
    }

    public ErrorOr<Success> UpdateDetails(string name, string description)
    {
        if (IsSystemRole)
        {
            return Error.Conflict("Role.SystemRoleImmutable", "لا يمكن تعديل بيانات الأدوار الأساسية للنظام.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Role.NameEmpty", "اسم الدور لا يمكن أن يكون فارغاً.");
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        return Result.Success;
    }

    public ErrorOr<Success> AssignPermission(int permissionId)
    {
        if (_permissions.Any(p => p.PermissionId == permissionId))
        {
            return Error.Conflict("Role.PermissionAlreadyAssigned", "هذه الصلاحية معينة بالفعل لهذا الدور.");
        }

        _permissions.Add(new RolePermission(Id, permissionId));
        return Result.Success;
    }

    public ErrorOr<Success> RemovePermission(int permissionId)
    {
        var existing = _permissions.FirstOrDefault(p => p.PermissionId == permissionId);
        if (existing is null)
        {
            return Error.NotFound("Role.PermissionNotFound", "الصلاحية غير موجودة ضمن هذا الدور.");
        }

        _permissions.Remove(existing);
        return Result.Success;
    }
}
