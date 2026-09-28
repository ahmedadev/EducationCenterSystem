namespace EducationCenterSystem.Presentation.WinForms.Theme;

public static class AppTheme
{
    // Color Palette matching Colors.xaml
    public static readonly Color BackgroundDark = ColorTranslator.FromHtml("#0F172A");
    public static readonly Color SurfaceCard = ColorTranslator.FromHtml("#1E293B");
    public static readonly Color SurfaceCardHover = ColorTranslator.FromHtml("#283548");
    public static readonly Color SurfaceHeader = ColorTranslator.FromHtml("#0F172A");
    public static readonly Color BorderSubtle = ColorTranslator.FromHtml("#334155");
    public static readonly Color BorderFocus = ColorTranslator.FromHtml("#38BDF8");
    
    // Accents
    public static readonly Color AccentPrimary = ColorTranslator.FromHtml("#0284C7");
    public static readonly Color AccentPrimaryHover = ColorTranslator.FromHtml("#0369A1");
    public static readonly Color AccentPrimaryPressed = ColorTranslator.FromHtml("#075985");

    // Semantic Status
    public static readonly Color StatusSuccess = ColorTranslator.FromHtml("#10B981");
    public static readonly Color StatusWarning = ColorTranslator.FromHtml("#F59E0B");
    public static readonly Color StatusDanger = ColorTranslator.FromHtml("#EF4444");

    // Typography Colors
    public static readonly Color TextPrimary = ColorTranslator.FromHtml("#F8FAFC");
    public static readonly Color TextSecondary = ColorTranslator.FromHtml("#94A3B8");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#64748B");
    public static readonly Color TextOnAccent = Color.White;

    // Typography Fonts
    public static readonly Font FontHero = new Font("Segoe UI", 18f, FontStyle.Bold);
    public static readonly Font FontTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FontSubtitle = new Font("Segoe UI", 12f, FontStyle.Bold);
    public static readonly Font FontBody = new Font("Segoe UI", 10f, FontStyle.Regular);
    public static readonly Font FontBodyBold = new Font("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font FontCaption = new Font("Segoe UI", 9f, FontStyle.Regular);
}
