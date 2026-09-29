namespace EducationCenterSystem.Presentation.WinForms.Models.DTOs;

public record GroupSessionDto(Guid Id, Guid GroupId, string GroupName, DateTime Date, string Notes);
