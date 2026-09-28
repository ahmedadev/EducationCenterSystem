namespace EducationCenterSystem.Presentation.Models;

public sealed class UserModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? LastLoginOnUtc { get; set; }
    public List<string> Roles { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayPhone => string.IsNullOrWhiteSpace(PhoneNumber) ? "غير مسجل" : PhoneNumber;
    public string PrimaryRole => Roles.FirstOrDefault() ?? "بدون دور";
    public string StatusBadge => IsActive ? "نشط" : "معطل";
    public string CreatedAtFormatted => CreatedOnUtc.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
    public string LastLoginFormatted => LastLoginOnUtc.HasValue 
        ? LastLoginOnUtc.Value.ToLocalTime().ToString("yyyy/MM/dd HH:mm") 
        : "لم يسجل بعد";
}
