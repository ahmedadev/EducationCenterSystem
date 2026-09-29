namespace EducationCenterSystem.Presentation.WinForms.Models.DTOs;

public record AttendanceRecordDto(Guid Id, Guid GroupSessionId, Guid StudentId, string StudentName, int Status, string Notes);
