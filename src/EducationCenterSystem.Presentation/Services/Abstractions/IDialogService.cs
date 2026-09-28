namespace EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

public interface IDialogService
{
    void ShowInfo(string message, string title);
    void ShowError(string message, string title);
    bool Confirm(string message, string title);
}
