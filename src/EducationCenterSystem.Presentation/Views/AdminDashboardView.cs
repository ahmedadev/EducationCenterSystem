using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs.Auth;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class AdminDashboardView : UserControl
{
    private readonly IUserApiService _userApiService;
    private readonly IRoleApiService _roleApiService;
    private readonly IDialogService _dialogService;
    private readonly IAuthApiService _authApiService;
    
    private StatCard _cardUsers = null!;
    private StatCard _cardActiveUsers = null!;
    private StatCard _cardRoles = null!;
    private StatCard _cardPermissions = null!;
    private DataGridView _gridUsers = null!;
    private AppButton _btnAddUser = null!;
    private AppButton _btnRefresh = null!;

    public AdminDashboardView(IDialogService dialogService, IAuthApiService authApiService, IUserApiService userApiService, IRoleApiService roleApiService)
    {
        _dialogService = dialogService;
        _authApiService = authApiService;
        _userApiService = userApiService;
        _roleApiService = roleApiService;

        InitializeComponent();



        _ = LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            var users = await _userApiService.GetAllUsersAsync();
            if (users != null)
            {
                var list = new List<object>();
                int activeCount = 0;
                foreach (var u in users)
                {
                    if (u.IsActive) activeCount++;
                list.Add(new
                {
                    u.Id,
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

        var roles = await _roleApiService.GetAllRolesAsync();
        if (roles != null)
        {
            _cardRoles.Value = roles.Count.ToString();
        }

        _cardPermissions.Value = "18";
    }
    catch (Exception ex)
    {
        _dialogService.ShowError($"خطأ أثناء جلب بيانات الإدارة: {ex.Message}", "خطأ");
    }
}

private async Task OpenChangeRoleDialogAsync()
{
    if (_gridUsers.SelectedRows.Count == 0) return;
    
    var selectedRow = _gridUsers.SelectedRows[0];
    if (selectedRow.DataBoundItem == null) return;
    
    var userIdProp = selectedRow.DataBoundItem.GetType().GetProperty("Id");
    if (userIdProp == null) return;
    
    var userId = (Guid)userIdProp.GetValue(selectedRow.DataBoundItem)!;

    List<RoleModel>? rolesList = null;
    try
    {
        rolesList = await _roleApiService.GetAllRolesAsync();
    }
    catch { }

    if (rolesList == null || rolesList.Count == 0)
    {
        _dialogService.ShowError("لا توجد أدوار متوفرة للنظام.", "تنبيه");
        return;
    }

    using var form = new Form
    {
        Text = "تغيير الصلاحية (الدور)",
        Size = new Size(350, 200),
        StartPosition = FormStartPosition.CenterParent,
        BackColor = AppTheme.BackgroundDark,
        ForeColor = AppTheme.TextPrimary,
        RightToLeft = RightToLeft.Yes,
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MaximizeBox = false,
        MinimizeBox = false
    };

    var pnlRole = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent, Padding = new Padding(10) };
    var lblRole = new Label { Text = "الدور (الصلاحيات) *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
    var cmbRoles = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
    
    cmbRoles.DataSource = rolesList;
    cmbRoles.DisplayMember = "Name";
    cmbRoles.ValueMember = "Id";

    pnlRole.Controls.Add(cmbRoles);
    pnlRole.Controls.Add(lblRole);

    var btnSave = new AppButton
    {
        Text = "حفظ التعديل",
        Variant = ButtonVariant.Primary,
        Dock = DockStyle.Bottom,
        Height = 40
    };

    btnSave.Click += async (s, e) =>
    {
        if (cmbRoles.SelectedValue is Guid roleId)
        {
            try
            {
                var success = await _userApiService.AssignRoleAsync(userId, roleId);
                if (success)
                {
                    _dialogService.ShowInfo("تم تغيير صلاحية المستخدم بنجاح", "نجاح");
                    form.Close();
                    await LoadDashboardDataAsync();
                }
                else
                {
                    _dialogService.ShowError("حدث خطأ أثناء تغيير الصلاحية", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ: {ex.Message}", "خطأ");
            }
        }
    };

    form.Controls.Add(pnlRole);
    form.Controls.Add(btnSave);

    form.ShowDialog(this);
}

private async Task OpenAddUserDialogAsync()
    {
        List<RoleModel>? rolesList = null;
        try
        {
            rolesList = await _roleApiService.GetAllRolesAsync();
        }
        catch { }

        using var form = new Form
        {
            Text = "إضافة مستخدم جديد",
            Size = new Size(420, 520),
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
        var phoneNumber = new FormField { LabelText = "رقم الهاتف", Dock = DockStyle.Top };
        var password = new FormField { LabelText = "كلمة المرور *", IsPassword = true, Dock = DockStyle.Top };

        var pnlRole = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
        var lblRole = new Label { Text = "الدور (الصلاحيات) *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
        var cmbRoles = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
        pnlRole.Controls.Add(cmbRoles);
        pnlRole.Controls.Add(lblRole);

        if (rolesList != null && rolesList.Count > 0)
        {
            cmbRoles.DataSource = rolesList;
            cmbRoles.DisplayMember = "Name";
            cmbRoles.ValueMember = "Id";
        }
        else
        {
            cmbRoles.Items.Add("لا توجد أدوار متوفرة");
            cmbRoles.SelectedIndex = 0;
            cmbRoles.Enabled = false;
        }

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

            var request = new RegisterRequest
            {
                FirstName = firstName.Value.Trim(),
                LastName = lastName.Value.Trim(),
                Email = email.Value.Trim(),
                Password = password.Value.Trim(),
                PhoneNumber = phoneNumber.Value.Trim(),
                RoleId = cmbRoles.SelectedValue as Guid?
            };

            try
            {
                // Use IAuthApiService for registering users like Clean Architecture specifies
                var result = await _authApiService.RegisterAsync(request);
                
                if (result)
                {
                    _dialogService.ShowInfo("تم إنشاء المستخدم وتعيين الصلاحيات بنجاح", "نجاح");
                    form.Close();
                    await LoadDashboardDataAsync();
                }
                else
                {
                    _dialogService.ShowError("فشل الإنشاء: يرجى التحقق من البيانات والمحاولة مرة أخرى", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ: {ex.Message}", "خطأ");
            }
        };

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };
        
        container.Controls.Add(pnlRole);
        container.Controls.Add(password);
        container.Controls.Add(phoneNumber);
        container.Controls.Add(email);
        container.Controls.Add(lastName);
        container.Controls.Add(firstName);

        form.Controls.Add(container);
        form.Controls.Add(btnSave); // Ensure it stays at the bottom
        
        form.ShowDialog(this);
    }
}
