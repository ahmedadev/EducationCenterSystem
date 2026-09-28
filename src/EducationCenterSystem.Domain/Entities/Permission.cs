namespace EducationCenterSystem.Domain.Entities;

public sealed class Permission
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Module { get; private set; }

    // Predefined System Permissions
    public const string StudentsRead = "Permissions.Students.Read";
    public const string StudentsCreate = "Permissions.Students.Create";
    public const string StudentsUpdate = "Permissions.Students.Update";
    public const string StudentsDelete = "Permissions.Students.Delete";

    public const string TeachersRead = "Permissions.Teachers.Read";
    public const string TeachersCreate = "Permissions.Teachers.Create";
    public const string TeachersUpdate = "Permissions.Teachers.Update";
    public const string TeachersDelete = "Permissions.Teachers.Delete";

    public const string AttendanceRead = "Permissions.Attendance.Read";
    public const string AttendanceMark = "Permissions.Attendance.Mark";

    public const string FinanceRead = "Permissions.Finance.Read";
    public const string FinanceManage = "Permissions.Finance.Manage";

    public const string UsersRead = "Permissions.Users.Read";
    public const string UsersManage = "Permissions.Users.Manage";
    public const string RolesManage = "Permissions.Roles.Manage";

    private Permission()
    {
        Name = default!;
        Description = default!;
        Module = default!;
    }

    public Permission(int id, string name, string description, string module)
    {
        Id = id;
        Name = name;
        Description = description;
        Module = module;
    }

    public static IReadOnlyList<Permission> GetPredefinedPermissions() =>
    [
        new(1, StudentsRead, "صلاحية استعراض بيانات الطلاب", "Students"),
        new(2, StudentsCreate, "صلاحية إضافة طالب جديد", "Students"),
        new(3, StudentsUpdate, "صلاحية تعديل بيانات الطلاب", "Students"),
        new(4, StudentsDelete, "صلاحية حذف أو أرشفة الطلاب", "Students"),

        new(5, TeachersRead, "صلاحية استعراض بيانات المدرسين", "Teachers"),
        new(6, TeachersCreate, "صلاحية إضافة مدرس جديد", "Teachers"),
        new(7, TeachersUpdate, "صلاحية تعديل بيانات المدرسين", "Teachers"),
        new(8, TeachersDelete, "صلاحية حذف أو أرشفة المدرسين", "Teachers"),

        new(9, AttendanceRead, "صلاحية استعراض سجلات الحضور", "Attendance"),
        new(10, AttendanceMark, "صلاحية تسجيل حضور وغياب الطلاب", "Attendance"),

        new(11, FinanceRead, "صلاحية استعراض الحسابات والمصروفات", "Finance"),
        new(12, FinanceManage, "صلاحية تسجيل المدفوعات والاشتراكات المالية", "Finance"),

        new(13, UsersRead, "صلاحية استعراض حسابات المستخدمين", "Users"),
        new(14, UsersManage, "صلاحية إنشاء وإدارة المستخدمين", "Users"),
        new(15, RolesManage, "صلاحية إدارة الأدوار وتعيين الصلاحيات", "Roles")
    ];
}
