using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs.Auth;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class RegisterView : UserControl
{
    private readonly IAuthApiService _authApiService;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;

    public event EventHandler? BackToLoginRequested;

    private FormField _firstNameField = null!;
    private FormField _lastNameField = null!;
    private FormField _emailField = null!;
    private FormField _passwordField = null!;
    private FormField _phoneField = null!;

    private AppButton _btnRegister = null!;
    private AppButton _btnBack = null!;
    private Label _errorLabel = null!;

    public RegisterView(IAuthApiService authApiService, IDialogService dialogService, IServiceProvider serviceProvider)
    {
        _authApiService = authApiService;
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;

        InitializeComponent();
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
            var success = await _authApiService.RegisterAsync(new RegisterRequest
            {
                FirstName = fn,
                LastName = ln,
                Email = email,
                Password = password,
                PhoneNumber = phone,
                RoleId = null
            });

            if (success)
            {
                _dialogService.ShowInfo("تم إنشاء الحساب بنجاح! يمكنك الآن تسجيل الدخول.", "نجاح");
                BackToLoginRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
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
