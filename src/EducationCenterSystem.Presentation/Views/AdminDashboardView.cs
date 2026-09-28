using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class AdminDashboardView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly StatCard _cardUsers;
    private readonly StatCard _cardActiveUsers;
    private readonly StatCard _cardRoles;
    private readonly StatCard _cardPermissions;
    private readonly DataGridView _gridUsers;
    private readonly AppButton _btnAddUser;
    private readonly AppButton _btnRefresh;

    public AdminDashboardView(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        // Top Header
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var titleLabel = new Label
        {
            Text = "لوحة إدارة النظام والصلاحيات",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(16, 10)
        };

        _btnAddUser = new AppButton
        {
            Text = "+ إضافة مستخدم",
            Variant = ButtonVariant.Primary,
            Width = 140,
            Height = 36,
            Location = new Point(16, 16)
        };
        _btnAddUser.Click += (s, e) => OpenAddUserDialog();

        _btnRefresh = new AppButton
        {
            Text = "تحديث",
            Variant = ButtonVariant.Secondary,
            Width = 90,
            Height = 36,
            Location = new Point(165, 16)
        };
        _btnRefresh.Click += async (s, e) => await LoadDashboardDataAsync();

        topPanel.Controls.Add(titleLabel);
        topPanel.Controls.Add(_btnAddUser);
        topPanel.Controls.Add(_btnRefresh);

        // Stats Cards Bar
        var statsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 95,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(16, 8, 16, 8),
            WrapContents = false
        };

        _cardUsers = new StatCard { Title = "إجمالي المستخدمين", Value = "0", ValueColor = AppTheme.TextPrimary };
        _cardActiveUsers = new StatCard { Title = "حسابات نشطة", Value = "0", ValueColor = AppTheme.StatusSuccess };
        _cardRoles = new StatCard { Title = "الأدوار المعرفة", Value = "0", ValueColor = AppTheme.AccentPrimary };
        _cardPermissions = new StatCard { Title = "إجمالي الصلاحيات", Value = "0", ValueColor = AppTheme.StatusWarning };

        statsPanel.Controls.Add(_cardUsers);
        statsPanel.Controls.Add(_cardActiveUsers);
        statsPanel.Controls.Add(_cardRoles);
        statsPanel.Controls.Add(_cardPermissions);

        // Users Grid
        _gridUsers = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            GridColor = AppTheme.BorderSubtle,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false
        };

        _gridUsers.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _gridUsers.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        _gridUsers.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBodyBold;
        _gridUsers.ColumnHeadersHeight = 38;

        _gridUsers.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _gridUsers.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        _gridUsers.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
        _gridUsers.DefaultCellStyle.SelectionForeColor = AppTheme.TextOnAccent;
        _gridUsers.DefaultCellStyle.Font = AppTheme.FontBody;
        _gridUsers.RowTemplate.Height = 36;

        _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 60 });
        _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "البريد الإلكتروني", DataPropertyName = "Email", FillWeight = 70 });
        _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "رقم الهاتف", DataPropertyName = "PhoneNumber", FillWeight = 40 });
        _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الدور", DataPropertyName = "RoleName", FillWeight = 40 });
        _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحالة", DataPropertyName = "StatusText", FillWeight = 30 });

        Controls.Add(_gridUsers);
        Controls.Add(statsPanel);
        Controls.Add(topPanel);

        _ = LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync("api/users");
            if (res.IsSuccessStatusCode)
            {
                var users = await res.Content.ReadFromJsonAsync<List<UserModel>>();
                if (users != null)
                {
                    var list = new List<object>();
                    int activeCount = 0;
                    foreach (var u in users)
                    {
                        if (u.IsActive) activeCount++;
                        list.Add(new
                        {
                            FullName = $"{u.FirstName} {u.LastName}".Trim(),
                            u.Email,
                            u.PhoneNumber,
                            RoleName = u.PrimaryRole,
                            StatusText = u.IsActive ? "نشط" : "معطل"
                        });
                    }

                    _gridUsers.DataSource = list;
                    _cardUsers.Value = users.Count.ToString();
                    _cardActiveUsers.Value = activeCount.ToString();
                }
            }

            var rolesRes = await _httpClient.GetAsync("api/roles");
            if (rolesRes.IsSuccessStatusCode)
            {
                var roles = await rolesRes.Content.ReadFromJsonAsync<List<RoleModel>>();
                _cardRoles.Value = (roles?.Count ?? 0).ToString();
            }

            _cardPermissions.Value = "18";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ أثناء جلب بيانات الإدارة: {ex.Message}", "خطأ");
        }
    }

    private void OpenAddUserDialog()
    {
        using var form = new Form
        {
            Text = "إضافة مستخدم جديد",
            Size = new Size(420, 420),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            RightToLeft = RightToLeft.Yes,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var firstName = new FormField { LabelText = "الاسم الأول *", Dock = DockStyle.Top };
        var lastName = new FormField { LabelText = "الاسم الأخير *", Dock = DockStyle.Top };
        var email = new FormField { LabelText = "البريد الإلكتروني *", Dock = DockStyle.Top };
        var password = new FormField { LabelText = "كلمة المرور *", IsPassword = true, Dock = DockStyle.Top };

        var btnSave = new AppButton
        {
            Text = "إنشاء الحساب",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Bottom,
            Height = 40
        };

        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(email.Value) || string.IsNullOrWhiteSpace(password.Value))
            {
                _dialogService.ShowError("الرجاء إدخال البريد الإلكتروني وكلمة المرور", "تنبيه");
                return;
            }

            var payload = new
            {
                firstName = firstName.Value.Trim(),
                lastName = lastName.Value.Trim(),
                email = email.Value.Trim(),
                password = password.Value.Trim()
            };

            try
            {
                var res = await _httpClient.PostAsJsonAsync("api/auth/register", payload);
                if (res.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم إنشاء المستخدم بنجاح", "نجاح");
                    form.Close();
                    await LoadDashboardDataAsync();
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    _dialogService.ShowError($"فشل الإنشاء: {err}", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ: {ex.Message}", "خطأ");
            }
        };

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        container.Controls.Add(password);
        container.Controls.Add(email);
        container.Controls.Add(lastName);
        container.Controls.Add(firstName);
        container.Controls.Add(btnSave);

        form.Controls.Add(container);
        form.ShowDialog(this);
    }
}
