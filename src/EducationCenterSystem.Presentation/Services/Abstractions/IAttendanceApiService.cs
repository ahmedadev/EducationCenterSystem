namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IAttendanceApiService
{
    Task<bool> SubmitBatchAttendanceAsync(object payload);
}
