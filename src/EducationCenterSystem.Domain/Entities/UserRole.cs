namespace EducationCenterSystem.Domain.Entities;

public sealed class UserRole
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }

    private UserRole()
    {
        User = default!;
        Role = default!;
    }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
        User = default!;
        Role = default!;
    }
}
