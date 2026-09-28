using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;
using EducationCenterSystem.Presentation.WinForms.Views;

namespace EducationCenterSystem.Presentation.WinForms;

public class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITokenProvider _tokenProvider;
    private readonly Panel _contentPanel;
    private readonly FlowLayoutPanel _navTabsPanel;
    private readonly Label _userNameLabel;
    private readonly Label _statusBadge;
    private AppButton? _activeNavButton;

    public MainForm(IServiceProvider serviceProvider, ITokenProvider tokenProvider)
    {
        _serviceProvider = serviceProvider;
        _tokenProvider = tokenProvider;

        Text = "EducationCenterSystem - منصة إدارة المراكز التعليمية";
        Size = new Size(1280, 800);
        MinimumSize = new Size(1024, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;

        // Top Navigation & Header Bar
        var topBar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12),
            ColumnCount = 3,
            RowCount = 1,
            RightToLeft = RightToLeft.Yes
        };
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260f));
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260f));

        // System Logo / Title
        var titlePanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        var appTitle = new Label
        {
            Text = "منصة الإدارة التعليمية",
            Font = AppTheme.FontSubtitle,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(0, 4),
            AutoSize = true
        };

        var appSub = new Label
        {
            Text = "Education Center System",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.AccentPrimary,
            Location = new Point(0, 32),
            AutoSize = true
        };

        titlePanel.Controls.Add(appTitle);
        titlePanel.Controls.Add(appSub);

        // User Chip & Status
        var userPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        _userNameLabel = new Label
        {
            Text = "مدير النظام",
            Font = AppTheme.FontBodyBold,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(0, 4),
            AutoSize = true
        };

        _statusBadge = new Label
        {
            Text = "● متصل بالسيرفر",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.StatusSuccess,
            Location = new Point(0, 32),
            AutoSize = true
        };

        var themeToggle = new AppButton
        {
            Text = "فاتح ☀️",
            Variant = ButtonVariant.Secondary,
            Width = 90,
            Height = 32,
            Location = new Point(160, 12)
        };
        themeToggle.Click += (s, e) => 
        {
            AppTheme.SetTheme(!AppTheme.IsDarkMode);
            themeToggle.Text = AppTheme.IsDarkMode ? "فاتح ☀️" : "داكن 🌙";
            
            BackColor = AppTheme.BackgroundDark;
            topBar.BackColor = AppTheme.SurfaceCard;
            _contentPanel.BackColor = AppTheme.BackgroundDark;
            appTitle.ForeColor = AppTheme.TextPrimary;
            _userNameLabel.ForeColor = AppTheme.TextPrimary;
            
            foreach (Control ctrl in _navTabsPanel.Controls) ctrl.Invalidate();
            _activeNavButton?.PerformClick();
        };

        userPanel.Controls.Add(_userNameLabel);
        userPanel.Controls.Add(_statusBadge);
        userPanel.Controls.Add(themeToggle);

        // Navigation Tabs in Center
        _navTabsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight, // Under RightToLeft=Yes, LeftToRight physically starts from the Right edge
            Padding = new Padding(10, 4, 10, 4)
        };

        CreateNavTab("الطلاب", () => _serviceProvider.GetRequiredService<StudentsView>(), true);
        CreateNavTab("المعلمين", () => _serviceProvider.GetRequiredService<TeachersView>());
        CreateNavTab("المواد والمجموعات", () => _serviceProvider.GetRequiredService<CoursesAndGroupsView>());
        CreateNavTab("الحضور والغياب", () => _serviceProvider.GetRequiredService<AttendanceView>());
        CreateNavTab("لوحة الإدارة", () => _serviceProvider.GetRequiredService<AdminDashboardView>());

        topBar.Controls.Add(titlePanel, 0, 0);
        topBar.Controls.Add(_navTabsPanel, 1, 0);
        topBar.Controls.Add(userPanel, 2, 0);

        // Main Content View Container
        _contentPanel = new BufferedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundDark
        };

        Controls.Add(_contentPanel);
        Controls.Add(topBar);

        // Auto authenticate and load initial view
        Shown += async (s, e) =>
        {
            await AuthenticateDefaultAdminAsync();
            ShowView(_serviceProvider.GetRequiredService<StudentsView>());
        };
    }

    private void CreateNavTab(string text, Func<UserControl> viewFactory, bool isInitial = false)
    {
        var btn = new AppButton
        {
            Text = text,
            Variant = ButtonVariant.NavTab,
            Width = 140,
            Height = 38,
            Margin = new Padding(6, 0, 6, 0),
            IsActive = isInitial
        };

        if (isInitial)
        {
            _activeNavButton = btn;
        }

        btn.Click += (s, e) =>
        {
            if (_activeNavButton != null)
            {
                _activeNavButton.IsActive = false;
            }

            btn.IsActive = true;
            _activeNavButton = btn;

            var view = viewFactory();
            ShowView(view);
        };

        _navTabsPanel.Controls.Add(btn);
    }

    private void ShowView(UserControl view)
    {
        _contentPanel.SuspendLayout();
        
        // Dispose old views to free resources and prevent UI ghosting
        foreach (Control ctrl in _contentPanel.Controls)
        {
            ctrl.Dispose();
        }
        
        _contentPanel.Controls.Clear();
        view.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(view);
        _contentPanel.ResumeLayout();
        _contentPanel.Invalidate(); // Force a clean repaint
    }

    private async Task AuthenticateDefaultAdminAsync()
    {
        try
        {
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("api/auth/login", new
            {
                email = "admin@educationcenter.com",
                password = "Admin123456!"
            });

            if (loginResponse.IsSuccessStatusCode)
            {
                var authResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                string token = authResult.TryGetProperty("token", out var tProp) ? (tProp.GetString() ?? string.Empty) : string.Empty;
                string firstName = authResult.TryGetProperty("firstName", out var fnProp) ? (fnProp.GetString() ?? string.Empty) : string.Empty;
                string lastName = authResult.TryGetProperty("lastName", out var lnProp) ? (lnProp.GetString() ?? string.Empty) : string.Empty;
                string email = authResult.TryGetProperty("email", out var emProp) ? (emProp.GetString() ?? string.Empty) : string.Empty;

                _tokenProvider.SetAuthentication(token, $"{firstName} {lastName}".Trim(), email, new List<string> { "Admin" }, new List<string>());
                _userNameLabel.Text = _tokenProvider.CurrentUserName;
                _statusBadge.Text = "● متصل كمسؤول";
                _statusBadge.ForeColor = AppTheme.StatusSuccess;
            }
            else
            {
                _statusBadge.Text = "● وضع عدم الاتصال";
                _statusBadge.ForeColor = AppTheme.StatusWarning;
            }
        }
        catch
        {
            _statusBadge.Text = "● الخادم غير متاح";
            _statusBadge.ForeColor = AppTheme.StatusDanger;
        }
    }

    // Specialized panel to prevent flickering and paint remnants during navigation
    private class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
