using EducationCenterSystem.Domain.Events;
using EducationCenterSystem.Domain.Primitives;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;

namespace EducationCenterSystem.Domain.Entities;

public sealed class User : AggregateRoot
{
    private readonly List<UserRole> _userRoles = new();

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? LastLoginOnUtc { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() : base()
    {
        FirstName = default!;
        LastName = default!;
        Email = default!;
        PasswordHash = default!;
    }

    private User(
        Guid id,
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        PhoneNumber? phoneNumber,
        DateTime createdOnUtc) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        IsActive = true;
        CreatedOnUtc = createdOnUtc;
    }

    public static ErrorOr<User> Create(
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        PhoneNumber? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Error.Validation("User.FirstNameEmpty", "الاسم الأول مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Error.Validation("User.LastNameEmpty", "الاسم الأخير مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Error.Validation("User.PasswordHashEmpty", "كلمة المرور المشفرة مطلوبة.");
        }

        var user = new User(
            Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            email,
            passwordHash,
            phoneNumber,
            DateTime.UtcNow);

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value));

        return user;
    }

    public ErrorOr<Success> AssignRole(Guid roleId)
    {
        if (_userRoles.Any(ur => ur.RoleId == roleId))
        {
            return Error.Conflict("User.RoleAlreadyAssigned", "هذا الدور معين بالفعل لهذا المستخدم.");
        }

        _userRoles.Add(new UserRole(Id, roleId));
        return Result.Success;
    }

    public ErrorOr<Success> RemoveRole(Guid roleId)
    {
        var existing = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (existing is null)
        {
            return Error.NotFound("User.RoleNotAssigned", "هذا الدور غير مرتبط بهذا المستخدم.");
        }

        _userRoles.Remove(existing);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateProfile(string firstName, string lastName, PhoneNumber? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Error.Validation("User.FirstNameEmpty", "الاسم الأول مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Error.Validation("User.LastNameEmpty", "الاسم الأخير مطلوب.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber;

        return Result.Success;
    }

    public ErrorOr<Success> ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return Error.Validation("User.PasswordHashEmpty", "كلمة المرور المشفرة مطلوبة.");
        }

        PasswordHash = newPasswordHash;
        return Result.Success;
    }

    public void RecordLogin(DateTime utcNow)
    {
        LastLoginOnUtc = utcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
