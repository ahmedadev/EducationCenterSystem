using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class RegisterView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;

    public event EventHandler? BackToLoginRequested;

    private readonly FormField _firstNameField;
    private readonly FormField _lastNameField;
    private readonly FormField _emailField;
    private readonly FormField _passwordField;
    private readonly FormField _phoneField;

    private readonly AppButton _btnRegister;
    private readonly AppButton _btnBack;
    private readonly Label _errorLabel;

    public RegisterView(IHttpClientFactory httpClientFactory, IDialogService dialogService, IServiceProvider serviceProvider)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        var card = new Panel
        {
            Width = 450,
            Height = 650,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(40)
        };

        Resize += (s, e) =>
        {
            card.Left = (Width - card.Width) / 2;
            card.Top = (Height - card.Height) / 2;
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

        Controls.Add(card);
    }

    private async Task PerformRegisterAsync()
    {
        var fn = _firstNameField.Value.Trim();
        var ln = _lastNameField.Value.Trim();
        var email = _emailField.Value.Trim();
        var phone = _phoneField.Value.Trim();
        var password = _passwordField.Value.Trim();

        if (string.IsNullOrWhiteSpace(fn) || string.IsNullOrWhiteSpace(ln) ||
            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _errorLabel.Text = "يرجى ملء جميع الحقول الأساسية";
            return;
        }

        _btnRegister.Enabled = false;
        _btnRegister.Text = "جاري التسجيل...";
        _errorLabel.Text = "";

        try
        {
            var payload = new
            {
                firstName = fn,
                lastName = ln,
                email = email,
                password = password,
                phoneNumber = phone,
                roleId = (Guid?)null // Wait, does the API need a valid RoleId? By default maybe it works, or we need to pass a default role. We will test.
            };

            var res = await _httpClient.PostAsJsonAsync("api/auth/register", payload);

            if (res.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo("تم إنشاء الحساب بنجاح! يمكنك الآن تسجيل الدخول.", "نجاح");
                BackToLoginRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                var error = await res.Content.ReadAsStringAsync();
                _errorLabel.Text = "حدث خطأ أثناء التسجيل. ربما البريد مستخدم؟";
            }
        }
        catch (Exception ex)
        {
            _errorLabel.Text = "لا يمكن الاتصال بالخادم";
            _dialogService.ShowError(ex.Message, "خطأ");
        }
        finally
        {
            _btnRegister.Enabled = true;
            _btnRegister.Text = "إنشاء حساب";
        }
    }
}
