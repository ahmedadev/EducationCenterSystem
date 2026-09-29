using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class LoginView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ITokenProvider _tokenProvider;

    public event EventHandler<(string Name, string Role)>? LoginSuccessful;
    public event EventHandler? RegisterRequested;

    private readonly FormField _emailField;
    private readonly FormField _passwordField;
    private readonly AppButton _btnLogin;
    private readonly Label _errorLabel;

    public LoginView(IHttpClientFactory httpClientFactory, IDialogService dialogService, ITokenProvider tokenProvider)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
        _tokenProvider = tokenProvider;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        // Center Panel (Glassmorphism inspired Login Card)
        var card = new Panel
        {
            Width = 400,
            Height = 450,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(40)
        };

        // Center the card
        Resize += (s, e) =>
        {
            card.Left = (Width - card.Width) / 2;
            card.Top = (Height - card.Height) / 2;
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
        // We need to set password char if possible, assuming FormField exposes the inner TextBox or we can just use normal TextBox here.
        // For now, FormField might not support PasswordChar, so we will use a raw TextBox for password.

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

        Controls.Add(card);
    }

    private async Task PerformLoginAsync()
    {
        var email = _emailField.Value.Trim();
        var password = _passwordField.Value.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _errorLabel.Text = "يرجى إدخال البريد الإلكتروني وكلمة المرور";
            return;
        }

        _btnLogin.Enabled = false;
        _btnLogin.Text = "جاري الدخول...";
        _errorLabel.Text = "";

        try
        {
            var loginResponse = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                email = email,
                password = password
            });

            if (loginResponse.IsSuccessStatusCode)
            {
                var authResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                string token = authResult.TryGetProperty("token", out var tProp) ? (tProp.GetString() ?? string.Empty) : string.Empty;
                string firstName = authResult.TryGetProperty("firstName", out var fnProp) ? (fnProp.GetString() ?? string.Empty) : string.Empty;
                string lastName = authResult.TryGetProperty("lastName", out var lnProp) ? (lnProp.GetString() ?? string.Empty) : string.Empty;
                string emailResult = authResult.TryGetProperty("email", out var emProp) ? (emProp.GetString() ?? string.Empty) : string.Empty;

                string fullName = $"{firstName} {lastName}".Trim();
                _tokenProvider.SetAuthentication(token, fullName, emailResult, new List<string> { "Admin" }, new List<string>());

                LoginSuccessful?.Invoke(this, (fullName, "مسؤول"));
            }
            else
            {
                _errorLabel.Text = "بيانات الدخول غير صحيحة";
            }
        }
        catch (Exception ex)
        {
            _errorLabel.Text = "لا يمكن الاتصال بالخادم";
            _dialogService.ShowError(ex.Message, "خطأ في الاتصال");
        }
        finally
        {
            _btnLogin.Enabled = true;
            _btnLogin.Text = "دخول";
        }
    }
}
