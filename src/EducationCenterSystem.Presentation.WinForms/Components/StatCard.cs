using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Components;

public class StatCard : Panel
{
    private readonly Label _titleLabel;
    private readonly Label _valueLabel;

    public string Title
    {
        get => _titleLabel.Text;
        set => _titleLabel.Text = value;
    }

    public string Value
    {
        get => _valueLabel.Text;
        set => _valueLabel.Text = value;
    }

    public Color ValueColor
    {
        get => _valueLabel.ForeColor;
        set => _valueLabel.ForeColor = value;
    }

    public StatCard()
    {
        Size = new Size(160, 80);
        BackColor = AppTheme.SurfaceCard;
        Padding = new Padding(12);

        _valueLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 32,
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _titleLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 22,
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.TextSecondary,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.Add(_titleLabel);
        Controls.Add(_valueLabel);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(AppTheme.BorderSubtle, 1f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }
}
