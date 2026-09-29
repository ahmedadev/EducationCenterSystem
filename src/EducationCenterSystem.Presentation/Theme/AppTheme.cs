namespace EducationCenterSystem.Presentation.WinForms.Theme;

public static class AppTheme
{
    public static bool IsDarkMode { get; private set; } = true;

    // Premium UI - 60-30-10 Law Colors
    public static Color BackgroundDark { get; private set; }
    public static Color SurfaceCard { get; private set; }
    public static Color SurfaceCardHover { get; private set; }
    public static Color SurfaceHeader { get; private set; }
    public static Color BorderSubtle { get; private set; }
    public static Color BorderFocus { get; private set; }

    // Accents
    public static Color AccentPrimary { get; private set; }
    public static Color AccentPrimaryHover { get; private set; }
    public static Color AccentPrimaryPressed { get; private set; }

    // Semantic Status
    public static Color StatusSuccess { get; private set; }
    public static Color StatusWarning { get; private set; }
    public static Color StatusDanger { get; private set; }

    // Typography Colors
    public static Color TextPrimary { get; private set; }
    public static Color TextSecondary { get; private set; }
    public static Color TextMuted { get; private set; }
    public static Color TextOnAccent { get; private set; }

    static AppTheme()
    {
        SetTheme(true); // Default to Dark
    }

    public static void SetTheme(bool isDark)
    {
        IsDarkMode = isDark;
        if (isDark)
        {
            BackgroundDark = ColorTranslator.FromHtml("#0B0F19");
            SurfaceCard = ColorTranslator.FromHtml("#111827");
            SurfaceCardHover = ColorTranslator.FromHtml("#1F2937");
            SurfaceHeader = ColorTranslator.FromHtml("#0B0F19");
            BorderSubtle = ColorTranslator.FromHtml("#374151");
            BorderFocus = ColorTranslator.FromHtml("#10B981");

            AccentPrimary = ColorTranslator.FromHtml("#059669");
            AccentPrimaryHover = ColorTranslator.FromHtml("#047857");
            AccentPrimaryPressed = ColorTranslator.FromHtml("#064E3B");

            StatusSuccess = ColorTranslator.FromHtml("#10B981");
            StatusWarning = ColorTranslator.FromHtml("#F59E0B");
            StatusDanger = ColorTranslator.FromHtml("#EF4444");

            TextPrimary = ColorTranslator.FromHtml("#F8FAFC");
            TextSecondary = ColorTranslator.FromHtml("#94A3B8");
            TextMuted = ColorTranslator.FromHtml("#475569");
            TextOnAccent = Color.White;
        }
        else
        {
            BackgroundDark = ColorTranslator.FromHtml("#F8FAFC"); // Light gray background
            SurfaceCard = ColorTranslator.FromHtml("#FFFFFF"); // White cards
            SurfaceCardHover = ColorTranslator.FromHtml("#F1F5F9");
            SurfaceHeader = ColorTranslator.FromHtml("#FFFFFF");
            BorderSubtle = ColorTranslator.FromHtml("#E2E8F0"); // Light border
            BorderFocus = ColorTranslator.FromHtml("#10B981");

            AccentPrimary = ColorTranslator.FromHtml("#059669");
            AccentPrimaryHover = ColorTranslator.FromHtml("#047857");
            AccentPrimaryPressed = ColorTranslator.FromHtml("#064E3B");

            StatusSuccess = ColorTranslator.FromHtml("#10B981");
            StatusWarning = ColorTranslator.FromHtml("#F59E0B");
            StatusDanger = ColorTranslator.FromHtml("#EF4444");

            TextPrimary = ColorTranslator.FromHtml("#0F172A"); // Dark text
            TextSecondary = ColorTranslator.FromHtml("#475569");
            TextMuted = ColorTranslator.FromHtml("#94A3B8");
            TextOnAccent = Color.White;
        }
    }

    // Typography Fonts (Using Segoe UI Variable / Segoe UI for robust Arabic rendering)
    public static readonly Font FontHero = new Font("Segoe UI", 22f, FontStyle.Bold);
    public static readonly Font FontTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
    public static readonly Font FontSubtitle = new Font("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FontBody = new Font("Segoe UI", 10.5f, FontStyle.Regular);
    public static readonly Font FontBodyBold = new Font("Segoe UI", 10.5f, FontStyle.Bold);
    public static readonly Font FontCaption = new Font("Segoe UI", 9f, FontStyle.Regular);
}
