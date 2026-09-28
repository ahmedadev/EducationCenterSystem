using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationCenterSystem.Presentation.Models;
using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.ViewModels;

public partial class AdminDashboardViewModel : ObservableValidator
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ITokenProvider _tokenProvider;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _busyMessage = "جاري التحميل...";

    [ObservableProperty]
    private int _selectedTabIndex = 0; // 0 = Roles & Permissions, 1 = Users

    // Stats
    [ObservableProperty]
    private int _totalUsersCount;

    [ObservableProperty]
    private int _activeUsersCount;

    [ObservableProperty]
    private int _totalRolesCount;

    [ObservableProperty]
    private int _totalPermissionsCount;

    // Collections
    public ObservableCollection<RoleModel> Roles { get; } = new();
    public ObservableCollection<PermissionItemModel> AvailablePermissions { get; } = new();
    public ObservableCollection<UserModel> Users { get; } = new();
    public ObservableCollection<UserModel> FilteredUsers { get; } = new();

    // Roles Management State
    [ObservableProperty]
    private RoleModel? _selectedRole;

    [ObservableProperty]
    private bool _isCreateRoleSectionOpen;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "اسم الدور مطلوب")]
    [MaxLength(50, ErrorMessage = "اسم الدور يجب ألا يتجاوز 50 حرفاً")]
    private string _newRoleName = string.Empty;

    [ObservableProperty]
    private string _newRoleDescription = string.Empty;

    // Users Management State
    [ObservableProperty]
    private UserModel? _selectedUser;

    [ObservableProperty]
    private string _userSearchText = string.Empty;

    [ObservableProperty]
    private bool _isCreateUserSectionOpen;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "الاسم الأول مطلوب")]
    private string _newUserFirstName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "الاسم الأخير مطلوب")]
    private string _newUserLastName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    private string _newUserEmail = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
    private string _newUserPassword = string.Empty;

    [ObservableProperty]
    private string _newUserPhoneNumber = string.Empty;

    [ObservableProperty]
    private RoleModel? _newUserSelectedRole;

    // Assign Role Dialog State
    [ObservableProperty]
    private bool _isAssignRoleDialogOpen;

    [ObservableProperty]
    private UserModel? _userToAssignRole;

    [ObservableProperty]
    private RoleModel? _roleToAssign;

    public bool CanManageRoles => _tokenProvider.IsInRole("Admin") || _tokenProvider.HasPermission(Permission.RolesManage);
    public bool CanManageUsers => _tokenProvider.IsInRole("Admin") || _tokenProvider.HasPermission(Permission.UsersManage);

    public AdminDashboardViewModel(
        IHttpClientFactory httpClientFactory,
        IDialogService dialogService,
        ITokenProvider tokenProvider)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
        _tokenProvider = tokenProvider;

        _ = LoadInitialDataAsync();
    }

    [RelayCommand]
    public async Task LoadInitialDataAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        BusyMessage = "جاري تحميل بيانات لوحة الإدارة...";
        try
        {
            await Task.WhenAll(
                LoadRolesAndPermissionsInternalAsync(cancellationToken),
                LoadUsersInternalAsync(cancellationToken));

            UpdateStats();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"فشل تحميل بيانات لوحة الإدارة:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SwitchTab(string tabIndexString)
    {
        if (int.TryParse(tabIndexString, out int index))
        {
            SelectedTabIndex = index;
        }
    }

    #region Roles & Permissions Management

    private async Task LoadRolesAndPermissionsInternalAsync(CancellationToken cancellationToken)
    {
        // 1. Load Permissions
        var permissionsResponse = await _httpClient.GetFromJsonAsync<List<PermissionItemModel>>("api/roles/permissions", cancellationToken);
        AvailablePermissions.Clear();
        if (permissionsResponse is not null)
        {
            foreach (var perm in permissionsResponse)
            {
                AvailablePermissions.Add(perm);
            }
        }

        // 2. Load Roles
        var rolesResponse = await _httpClient.GetFromJsonAsync<List<RoleModel>>("api/roles", cancellationToken);
        Roles.Clear();
        if (rolesResponse is not null)
        {
            foreach (var role in rolesResponse)
            {
                Roles.Add(role);
            }

            if (SelectedRole is null && Roles.Count > 0)
            {
                SelectedRole = Roles[0];
            }
        }
    }

    [RelayCommand]
    private async Task RefreshRolesAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        BusyMessage = "جاري تحديث الأدوار والصلاحيات...";
        try
        {
            await LoadRolesAndPermissionsInternalAsync(cancellationToken);
            UpdateStats();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ في تحديث الأدوار:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleCreateRoleSection()
    {
        IsCreateRoleSectionOpen = !IsCreateRoleSectionOpen;
        if (!IsCreateRoleSectionOpen)
        {
            ClearRoleForm();
        }
    }

    [RelayCommand]
    private void SelectAllPermissions()
    {
        foreach (var perm in AvailablePermissions)
        {
            perm.IsSelected = true;
        }
    }

    [RelayCommand]
    private void ClearAllPermissions()
    {
        foreach (var perm in AvailablePermissions)
        {
            perm.IsSelected = false;
        }
    }

    [RelayCommand]
    private void ToggleModulePermissions(string module)
    {
        var modulePerms = AvailablePermissions.Where(p => string.Equals(p.Module, module, StringComparison.OrdinalIgnoreCase)).ToList();
        bool anyUnchecked = modulePerms.Any(p => !p.IsSelected);
        foreach (var perm in modulePerms)
        {
            perm.IsSelected = anyUnchecked;
        }
    }

    [RelayCommand]
    private async Task CreateRoleAsync(CancellationToken cancellationToken = default)
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            _dialogService.ShowInfo("يرجى التأكد من صحة اسم الدور وبياناته.", "تنبيه");
            return;
        }

        var selectedPermissionIds = AvailablePermissions
            .Where(p => p.IsSelected)
            .Select(p => p.Id)
            .ToList();

        if (selectedPermissionIds.Count == 0)
        {
            _dialogService.ShowInfo("يرجى اختيار صلاحية واحدة على الأقل لهذا الدور.", "تنبيه");
            return;
        }

        IsBusy = true;
        BusyMessage = "جاري إنشاء الدور الجديد...";
        try
        {
            var payload = new
            {
                Name = NewRoleName.Trim(),
                Description = NewRoleDescription.Trim(),
                PermissionIds = selectedPermissionIds
            };

            var response = await _httpClient.PostAsJsonAsync("api/roles", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo($"تم إنشاء الدور '{NewRoleName.Trim()}' بنجاح.", "تمت العملية بنجاح");
                ClearRoleForm();
                IsCreateRoleSectionOpen = false;
                await LoadRolesAndPermissionsInternalAsync(cancellationToken);
                UpdateStats();
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                _dialogService.ShowError($"فشل إنشاء الدور:\n{errorText}", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"حدث استثناء أثناء حفظ الدور:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearRoleForm()
    {
        NewRoleName = string.Empty;
        NewRoleDescription = string.Empty;
        ClearAllPermissions();
        ClearErrors();
    }

    #endregion

    #region Users Management

    private async Task LoadUsersInternalAsync(CancellationToken cancellationToken)
    {
        var usersResponse = await _httpClient.GetFromJsonAsync<List<UserModel>>("api/auth/users", cancellationToken);
        Users.Clear();
        if (usersResponse is not null)
        {
            foreach (var user in usersResponse)
            {
                Users.Add(user);
            }
        }

        ApplyUserFilter();
    }

    [RelayCommand]
    private async Task RefreshUsersAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        BusyMessage = "جاري تحديث قائمة المستخدمين...";
        try
        {
            await LoadUsersInternalAsync(cancellationToken);
            UpdateStats();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ في تحديث المستخدمين:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnUserSearchTextChanged(string value)
    {
        ApplyUserFilter();
    }

    private void ApplyUserFilter()
    {
        FilteredUsers.Clear();
        var query = UserSearchText?.Trim() ?? string.Empty;

        var filtered = string.IsNullOrWhiteSpace(query)
            ? Users.ToList()
            : Users.Where(u =>
                u.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                u.Roles.Any(r => r.Contains(query, StringComparison.OrdinalIgnoreCase))
            ).ToList();

        foreach (var user in filtered)
        {
            FilteredUsers.Add(user);
        }
    }

    [RelayCommand]
    private void ToggleCreateUserSection()
    {
        IsCreateUserSectionOpen = !IsCreateUserSectionOpen;
        if (!IsCreateUserSectionOpen)
        {
            ClearUserForm();
        }
    }

    [RelayCommand]
    private async Task CreateUserAsync(CancellationToken cancellationToken = default)
    {
        ValidateProperty(NewUserFirstName, nameof(NewUserFirstName));
        ValidateProperty(NewUserLastName, nameof(NewUserLastName));
        ValidateProperty(NewUserEmail, nameof(NewUserEmail));
        ValidateProperty(NewUserPassword, nameof(NewUserPassword));

        if (GetErrors(nameof(NewUserFirstName)).Any() ||
            GetErrors(nameof(NewUserLastName)).Any() ||
            GetErrors(nameof(NewUserEmail)).Any() ||
            GetErrors(nameof(NewUserPassword)).Any())
        {
            _dialogService.ShowInfo("يرجى التحقق من صحة بيانات المستخدم المطلوبة.", "تنبيه");
            return;
        }

        IsBusy = true;
        BusyMessage = "جاري إضافة المستخدم الجديد...";
        try
        {
            var payload = new
            {
                FirstName = NewUserFirstName.Trim(),
                LastName = NewUserLastName.Trim(),
                Email = NewUserEmail.Trim(),
                Password = NewUserPassword,
                PhoneNumber = string.IsNullOrWhiteSpace(NewUserPhoneNumber) ? null : NewUserPhoneNumber.Trim(),
                RoleId = NewUserSelectedRole?.Id
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo($"تم إنشاء حساب المستخدم '{NewUserFirstName} {NewUserLastName}' بنجاح.", "تمت العملية بنجاح");
                ClearUserForm();
                IsCreateUserSectionOpen = false;
                await LoadUsersInternalAsync(cancellationToken);
                UpdateStats();
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                _dialogService.ShowError($"فشل إنشاء المستخدم:\n{errorText}", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"حدث استثناء أثناء تسجيل المستخدم:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearUserForm()
    {
        NewUserFirstName = string.Empty;
        NewUserLastName = string.Empty;
        NewUserEmail = string.Empty;
        NewUserPassword = string.Empty;
        NewUserPhoneNumber = string.Empty;
        NewUserSelectedRole = null;
        ClearErrors();
    }

    #endregion

    #region Role Assignment Dialog

    [RelayCommand]
    private void OpenAssignRoleDialog(UserModel? user)
    {
        if (user is null) return;
        UserToAssignRole = user;

        // Select currently assigned role if matching
        var currentRoleName = user.Roles.FirstOrDefault();
        RoleToAssign = Roles.FirstOrDefault(r => string.Equals(r.Name, currentRoleName, StringComparison.OrdinalIgnoreCase)) 
                       ?? Roles.FirstOrDefault();

        IsAssignRoleDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAssignRoleDialog()
    {
        IsAssignRoleDialogOpen = false;
        UserToAssignRole = null;
        RoleToAssign = null;
    }

    [RelayCommand]
    private async Task ConfirmAssignRoleAsync(CancellationToken cancellationToken = default)
    {
        if (UserToAssignRole is null || RoleToAssign is null)
        {
            _dialogService.ShowInfo("يرجى اختيار الدور المراد تعيينه للمستخدم.", "تنبيه");
            return;
        }

        IsBusy = true;
        BusyMessage = "جاري تحديث دور المستخدم...";
        try
        {
            var payload = new
            {
                UserId = UserToAssignRole.Id,
                RoleId = RoleToAssign.Id
            };

            var response = await _httpClient.PostAsJsonAsync("api/roles/assign", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo($"تم تعيين الدور '{RoleToAssign.Name}' للمستخدم '{UserToAssignRole.FullName}' بنجاح.", "تم التعيين");
                CloseAssignRoleDialog();
                await LoadUsersInternalAsync(cancellationToken);
                UpdateStats();
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                _dialogService.ShowError($"فشل تعيين الدور:\n{errorText}", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"حدث استثناء أثناء تعيين الدور:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    private void UpdateStats()
    {
        TotalUsersCount = Users.Count;
        ActiveUsersCount = Users.Count(u => u.IsActive);
        TotalRolesCount = Roles.Count;
        TotalPermissionsCount = AvailablePermissions.Count;
    }
}

