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

#if DEBUG
        // Read credentials from dev-credentials.txt if it exists
        try
        {
            var rootDir = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (rootDir != null && !System.IO.File.Exists(System.IO.Path.Combine(rootDir.FullName, "EducationCenterSystem.sln")))
            {
                rootDir = rootDir.Parent;
            }

            if (rootDir != null)
            {
                var credFile = System.IO.Path.Combine(rootDir.FullName, "dev-credentials.txt");
                if (System.IO.File.Exists(credFile))
                {
                    var lines = System.IO.File.ReadAllLines(credFile);
                    var validAccounts = lines.Where(l => l.Contains("|")).ToList();
                    
                    if (validAccounts.Any())
                    {
                        var card = this.Controls[0];
                        var combo = new ComboBox
                        {
                            Dock = DockStyle.Top,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Font = AppTheme.FontCaption,
                            Margin = new Padding(0, 0, 0, 15)
                        };

                        foreach (var acc in validAccounts)
                        {
                            var parts = acc.Split('|');
                            if (parts.Length >= 3)
                            {
                                combo.Items.Add(new { Name = parts[0], Email = parts[1], Password = parts[2] });
                            }
                        }

                        combo.DisplayMember = "Name";
                        combo.SelectedIndexChanged += (s, e) =>
                        {
                            if (combo.SelectedItem != null)
                            {
                                dynamic selected = combo.SelectedItem;
                                _emailField.Value = selected.Email;
                                _passwordField.Value = selected.Password;
                            }
                        };

                        card.Controls.Add(combo);
                        // Move it to just below the subtitle label (index wise)
                        card.Controls.SetChildIndex(combo, card.Controls.IndexOf(_emailField) + 1);

                        if (combo.Items.Count > 0)
                            combo.SelectedIndex = 0;
                    }
                }
            }
        }
        catch { }
#endif
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
