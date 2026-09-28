using System.ComponentModel;
using System.Drawing.Drawing2D;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Components;

public class AppButton : Button
{
    private ButtonVariant _variant = ButtonVariant.Primary;
    private bool _isHovered;
    private bool _isPressed;
    private bool _isActive;
    private int _borderRadius = 8;

    [Category("App Styling")]
    [DefaultValue(ButtonVariant.Primary)]
    public ButtonVariant Variant
    {
        get => _variant;
        set
        {
            _variant = value;
            ApplyStyle();
            Invalidate();
        }
    }

    [Category("App Styling")]
    [DefaultValue(false)]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Invalidate();
        }
    }

    [Category("App Styling")]
    [DefaultValue(8)]
    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    public AppButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Font = AppTheme.FontBodyBold;
        Cursor = Cursors.Hand;
        Height = 38;
        Padding = new Padding(12, 6, 12, 6);
        ApplyStyle();
    }

    private void ApplyStyle()
    {
        switch (_variant)
        {
            case ButtonVariant.Primary:
                ForeColor = AppTheme.TextOnAccent;
                break;
            case ButtonVariant.Secondary:
                ForeColor = AppTheme.TextPrimary;
                break;
            case ButtonVariant.NavTab:
                ForeColor = _isActive ? AppTheme.AccentPrimary : AppTheme.TextSecondary;
                break;
            case ButtonVariant.Success:
                ForeColor = Color.White;
                break;
            case ButtonVariant.Danger:
                ForeColor = Color.White;
                break;
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        _isPressed = true;
        Invalidate();
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        _isPressed = false;
        Invalidate();
        base.OnMouseUp(mevent);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        
        // Prevent ghosting/remnants by clearing the background with the actual parent solid color
        Color parentBg = BackColor;
        Control? currentParent = Parent;
        while (currentParent != null)
        {
            if (currentParent.BackColor != Color.Transparent)
            {
                parentBg = currentParent.BackColor;
                break;
            }
            currentParent = currentParent.Parent;
        }
        g.Clear(parentBg);

        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        Color bgColor;
        Color borderColor = Color.Transparent;
        Color textColor;

        switch (_variant)
        {
            case ButtonVariant.Primary:
                bgColor = _isPressed ? AppTheme.AccentPrimaryPressed : (_isHovered ? AppTheme.AccentPrimaryHover : AppTheme.AccentPrimary);
                textColor = AppTheme.TextOnAccent;
                break;

            case ButtonVariant.Secondary:
                bgColor = _isPressed ? AppTheme.SurfaceCard : (_isHovered ? AppTheme.BorderSubtle : AppTheme.SurfaceCardHover);
                borderColor = AppTheme.BorderSubtle;
                textColor = AppTheme.TextPrimary;
                break;

            case ButtonVariant.NavTab:
                if (_isActive)
                {
                    bgColor = AppTheme.SurfaceCardHover;
                    borderColor = AppTheme.AccentPrimary;
                    textColor = AppTheme.AccentPrimary;
                }
                else
                {
                    bgColor = _isHovered ? AppTheme.SurfaceCardHover : Color.Transparent;
                    borderColor = _isHovered ? AppTheme.BorderFocus : AppTheme.BorderSubtle;
                    textColor = _isHovered ? AppTheme.TextPrimary : AppTheme.TextSecondary;
                }
                break;

            case ButtonVariant.Success:
                bgColor = _isHovered ? Color.FromArgb(14, 165, 114) : AppTheme.StatusSuccess;
                textColor = Color.White;
                break;

            case ButtonVariant.Danger:
                bgColor = _isHovered ? Color.FromArgb(220, 38, 38) : AppTheme.StatusDanger;
                textColor = Color.White;
                break;

            default:
                bgColor = AppTheme.AccentPrimary;
                textColor = AppTheme.TextOnAccent;
                break;
        }

        if (!Enabled)
        {
            bgColor = Color.FromArgb(60, bgColor);
            textColor = Color.FromArgb(120, textColor);
        }

        // Draw background
        using (var path = GetRoundedRectangle(rect, _borderRadius))
        {
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            if (borderColor != Color.Transparent)
            {
                using (var pen = new Pen(borderColor, 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        // Draw text and image
        TextRenderer.DrawText(
            g,
            Text,
            Font,
            rect,
            textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
        );
    }

    private static GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int diameter = radius * 2;
        var size = new Size(diameter, diameter);
        var arc = new Rectangle(bounds.Location, size);

        // Top-left
        path.AddArc(arc, 180, 90);

        // Top-right
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // Bottom-right
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // Bottom-left
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }
}
