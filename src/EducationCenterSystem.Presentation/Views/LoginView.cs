using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs.Auth;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class LoginView : UserControl
{
    private readonly IAuthApiService _authApiService;
    private readonly IDialogService _dialogService;
    private readonly ITokenProvider _tokenProvider;

    public event EventHandler<(string Name, string Role)>? LoginSuccessful;
    public event EventHandler? RegisterRequested;

    private FormField _emailField = null!;
    private FormField _passwordField = null!;
    private AppButton _btnLogin = null!;
    private Label _errorLabel = null!;

    public LoginView(IAuthApiService authApiService, IDialogService dialogService, ITokenProvider tokenProvider)
    {
        _authApiService = authApiService;
        _dialogService = dialogService;
        _tokenProvider = tokenProvider;

        InitializeComponent();
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
            var result = await _authApiService.LoginAsync(new LoginRequest
            {
                Email = email,
                Password = password
            });

            if (result != null)
            {
                string fullName = $"{result.FirstName} {result.LastName}".Trim();
                _tokenProvider.SetAuthentication(result.Token, fullName, result.Email, new List<string> { "Admin" }, new List<string>());

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
