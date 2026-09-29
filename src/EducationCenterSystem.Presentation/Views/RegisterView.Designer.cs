using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class RegisterView
    {
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.BackgroundDark;
            this.RightToLeft = RightToLeft.Yes;

            var card = new Panel
            {
                Width = 450,
                Height = 650,
                BackColor = AppTheme.SurfaceCard,
                Padding = new Padding(40)
            };

            this.Resize += (s, e) =>
            {
                card.Left = (this.Width - card.Width) / 2;
                card.Top = (this.Height - card.Height) / 2;
            };

            var titleLabel = new Label
            {
                Text = "إنشاء حساب جديد",
                Font = AppTheme.FontHero,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "قم بتسجيل بيانات المستخدم الجديد",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 20)
            };

            _firstNameField = new FormField { LabelText = "الاسم الأول", Dock = DockStyle.Top, Height = 65 };
            _lastNameField = new FormField { LabelText = "الاسم الأخير", Dock = DockStyle.Top, Height = 65 };
            _emailField = new FormField { LabelText = "البريد الإلكتروني", Dock = DockStyle.Top, Height = 65 };
            _phoneField = new FormField { LabelText = "رقم الهاتف", Dock = DockStyle.Top, Height = 65 };
            _passwordField = new FormField { LabelText = "كلمة المرور", Dock = DockStyle.Top, Height = 65, IsPassword = true };

            _errorLabel = new Label
            {
                Text = "",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.StatusDanger,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 10)
            };

            _btnRegister = new AppButton
            {
                Text = "إنشاء حساب",
                Variant = ButtonVariant.Primary,
                Dock = DockStyle.Top,
                Height = 45,
                Margin = new Padding(0, 15, 0, 10)
            };

            _btnBack = new AppButton
            {
                Text = "العودة لتسجيل الدخول",
                Variant = ButtonVariant.Secondary,
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 10, 0, 0)
            };

            _btnRegister.Click += async (s, e) => await PerformRegisterAsync();
            _btnBack.Click += (s, e) => BackToLoginRequested?.Invoke(this, EventArgs.Empty);

            card.Controls.Add(_btnBack);
            card.Controls.Add(_btnRegister);
            card.Controls.Add(_errorLabel);
            card.Controls.Add(_passwordField);
            card.Controls.Add(_phoneField);
            card.Controls.Add(_emailField);
            card.Controls.Add(_lastNameField);
            card.Controls.Add(_firstNameField);
            card.Controls.Add(subtitleLabel);
            card.Controls.Add(titleLabel);

            this.Controls.Add(card);
        }
    }
}
