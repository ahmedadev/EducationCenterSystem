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
            Width = 80,
            Height = 32,
            Location = new Point(175, 12)
        };
        themeToggle.Click += (s, e) => 
        {
            AppTheme.SetTheme(!AppTheme.IsDarkMode);
            themeToggle.Text = AppTheme.IsDarkMode ? "فاتح ☀️" : "داكن 🌙";
            
            BackColor = AppTheme.BackgroundDark;
            topBar.BackColor = AppTheme.SurfaceCard;
            if (_contentPanel != null) _contentPanel.BackColor = AppTheme.BackgroundDark;
            appTitle.ForeColor = AppTheme.TextPrimary;
            if (_userNameLabel != null) _userNameLabel.ForeColor = AppTheme.TextPrimary;
            
            if (_navTabsPanel != null)
            {
                foreach (Control ctrl in _navTabsPanel.Controls) ctrl.Invalidate();
            }
            _activeNavButton?.PerformClick();
        };

        var logoutBtn = new AppButton
        {
            Text = "خروج 🚪",
            Variant = ButtonVariant.Danger,
            Width = 80,
            Height = 32,
            Location = new Point(85, 12)
        };
        logoutBtn.Click += (s, e) =>
        {
            _tokenProvider.Clear();
            topBar.Visible = false;
            ShowLoginScreen(topBar);
        };

        userPanel.Controls.Add(_userNameLabel);
        userPanel.Controls.Add(_statusBadge);
        userPanel.Controls.Add(themeToggle);
        userPanel.Controls.Add(logoutBtn);

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
        CreateNavTab("أولياء الأمور", () => _serviceProvider.GetRequiredService<ParentsView>());
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

        topBar.Visible = false;

        Shown += (s, e) =>
        {
            ShowLoginScreen(topBar);
        };
    }

    private void ShowLoginScreen(Control topBar)
    {
        var loginView = _serviceProvider.GetRequiredService<LoginView>();
        loginView.LoginSuccessful += (s, args) =>
        {
            topBar.Visible = true;
            _userNameLabel.Text = args.Name;
            _statusBadge.Text = $"● متصل كـ {args.Role}";
            _statusBadge.ForeColor = AppTheme.StatusSuccess;
            
            if (_navTabsPanel.Controls.Count > 0 && _navTabsPanel.Controls[0] is AppButton btn)
            {
                btn.PerformClick();
            }
            else
            {
                ShowView(_serviceProvider.GetRequiredService<StudentsView>());
            }
        };

        loginView.RegisterRequested += (s, args) =>
        {
            ShowRegisterScreen(topBar);
        };

        ShowView(loginView);
    }

    private void ShowRegisterScreen(Control topBar)
    {
        var registerView = _serviceProvider.GetRequiredService<RegisterView>();
        registerView.BackToLoginRequested += (s, args) =>
        {
            ShowLoginScreen(topBar);
        };
        ShowView(registerView);
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
