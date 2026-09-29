using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class LoginView
    {
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.BackgroundDark;
            this.RightToLeft = RightToLeft.Yes;

            // Center Panel (Glassmorphism inspired Login Card)
            var card = new ModernCard
            {
                Width = 400,
                Height = 450,
                BorderRadius = 16,
                Padding = new Padding(40)
            };

            // Center the card
            this.Resize += (s, e) =>
            {
                card.Left = (this.Width - card.Width) / 2;
                card.Top = (this.Height - card.Height) / 2;
            };

            var titleLabel = new Label
            {
                Text = "تسجيل الدخول",
                Font = AppTheme.FontHero,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "مرحباً بك في منصة الإدارة التعليمية",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 30)
            };

            _emailField = new FormField
            {
                LabelText = "البريد الإلكتروني",
                Dock = DockStyle.Top,
                Height = 70
            };

            _passwordField = new FormField
            {
                LabelText = "كلمة المرور",
                Dock = DockStyle.Top,
                Height = 70,
                IsPassword = true
            };

            _errorLabel = new Label
            {
                Text = "",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.StatusDanger,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 10, 0, 10)
            };

            _btnLogin = new AppButton
            {
                Text = "دخول",
                Variant = ButtonVariant.Primary,
                Dock = DockStyle.Top,
                Height = 45,
                Margin = new Padding(0, 20, 0, 0)
            };

            _btnLogin.Click += async (s, e) => await PerformLoginAsync();

            var btnGoRegister = new AppButton
            {
                Text = "حساب جديد؟ إنشاء حساب",
                Variant = ButtonVariant.Secondary,
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 10, 0, 0)
            };
            btnGoRegister.Click += (s, e) => RegisterRequested?.Invoke(this, EventArgs.Empty);

            card.Controls.Add(btnGoRegister);
            card.Controls.Add(_btnLogin);
            card.Controls.Add(_errorLabel);
            card.Controls.Add(_passwordField);
            card.Controls.Add(_emailField);
            card.Controls.Add(subtitleLabel);
            card.Controls.Add(titleLabel);

            this.Controls.Add(card);
        }
    }
}
