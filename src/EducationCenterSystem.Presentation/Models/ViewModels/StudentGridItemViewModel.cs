namespace EducationCenterSystem.Presentation.WinForms.Models.ViewModels;

public class StudentGridItemViewModel
{
    public Guid Id { get; set; }
    public int SerialNumber { get; set; }
    public string? StudentCode { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ParentPhoneNumber { get; set; }
    public string? GradeLevel { get; set; }
    public string? StatusText { get; set; }
}
