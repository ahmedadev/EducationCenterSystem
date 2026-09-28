using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Components;

public class FormField : Panel
{
    private readonly Label _label;
    private readonly TextBox _textBox;

    public string LabelText
    {
        get => _label.Text;
        set => _label.Text = value;
    }

    public string Value
    {
        get => _textBox.Text;
        set => _textBox.Text = value;
    }

    public bool IsPassword
    {
        get => _textBox.UseSystemPasswordChar;
        set => _textBox.UseSystemPasswordChar = value;
    }

    public TextBox TextBoxControl => _textBox;

    public FormField()
    {
        Height = 65;
        BackColor = Color.Transparent;

        _label = new Label
        {
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontCaption,
            TextAlign = ContentAlignment.MiddleRight
        };

        _textBox = new TextBox
        {
            Dock = DockStyle.Bottom,
            Height = 32,
            BackColor = AppTheme.SurfaceCard,
            ForeColor = AppTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.FontBody
        };

        Controls.Add(_textBox);
        Controls.Add(_label);
    }
}
