using System.Drawing.Drawing2D;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Components;

public class ModernCard : Panel
{
    private int _borderRadius = 16;
    private int _shadowSize = 5;

    public int BorderRadius
    {
        get => _borderRadius;
        set { _borderRadius = value; Invalidate(); }
    }

    public ModernCard()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Transparent;
        Padding = new Padding(_shadowSize + 4);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var rect = new Rectangle(_shadowSize, _shadowSize, Width - _shadowSize * 2, Height - _shadowSize * 2);
        
        using var path = GetRoundedPath(rect, _borderRadius);
        
        // Draw subtle shadow
        if (AppTheme.IsDarkMode)
        {
            using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
            var shadowRect = rect;
            shadowRect.Offset(2, 4);
            using var shadowPath = GetRoundedPath(shadowRect, _borderRadius);
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }
        else
        {
            using var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            var shadowRect = rect;
            shadowRect.Offset(0, 4);
            using var shadowPath = GetRoundedPath(shadowRect, _borderRadius);
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        // Draw card background
        using var brush = new SolidBrush(AppTheme.SurfaceCard);
        e.Graphics.FillPath(brush, path);
        
        // Draw subtle border
        using var pen = new Pen(AppTheme.BorderSubtle, 1);
        e.Graphics.DrawPath(pen, path);
    }

    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var d = radius * 2;
        
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        
        return path;
    }
}
