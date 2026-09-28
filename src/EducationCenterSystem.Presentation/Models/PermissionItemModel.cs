using CommunityToolkit.Mvvm.ComponentModel;

namespace EducationCenterSystem.Presentation.Models;

public partial class PermissionItemModel : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;

    public string ModuleDisplayName => Module switch
    {
        "Students" => "الطلاب",
        "Teachers" => "المعلمين",
        "Attendance" => "الحضور والغياب",
        "Finance" => "الحسابات والمالية",
        "Users" => "المستخدمين",
        "Roles" => "الأدوار والصلاحيات",
        _ => Module
    };
}
