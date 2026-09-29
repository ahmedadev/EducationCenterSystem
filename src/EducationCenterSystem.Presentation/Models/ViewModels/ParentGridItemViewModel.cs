namespace EducationCenterSystem.Presentation.WinForms.Models.ViewModels;

public sealed class ParentGridItemViewModel
{
    public Guid Id { get; set; }
    public int SerialNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Job { get; set; }
    public string? NationalId { get; set; }
    public int ChildrenCount { get; set; }
}
