using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public sealed class DialogService : IDialogService
{
    public void ShowInfo(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void ShowError(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public bool Confirm(string message, string title)
    {
        return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
