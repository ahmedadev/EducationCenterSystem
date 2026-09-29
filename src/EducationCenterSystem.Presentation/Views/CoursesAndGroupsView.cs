

using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class CoursesAndGroupsView : UserControl
{
    private readonly IDialogService _dialogService;
    private readonly ITeacherApiService _teacherApiService;
    private readonly ICourseApiService _courseApiService;
    private readonly IEducationalGroupApiService _educationalGroupApiService;

    private DataGridView _gridCourses = null!;
    private DataGridView _gridGroups = null!;
    private FormField _txtCourseName = null!;
    private FormField _txtGradeLevel = null!;
    private FormField _txtSubject = null!;
    private AppButton _btnSaveCourse = null!;
    private AppButton _btnRefresh = null!;

    private ComboBox _cmbCourses = null!;
    private ComboBox _cmbTeachers = null!;
    private FormField _txtGroupName = null!;
    private FormField _txtMaxCapacity = null!;
    private FormField _txtMonthlyFee = null!;
    private FormField _txtSchedule = null!;
    private AppButton _btnSaveGroup = null!;

    public CoursesAndGroupsView(IDialogService dialogService, 
                                ITeacherApiService teacherApiService,
                                ICourseApiService courseApiService,
                                IEducationalGroupApiService educationalGroupApiService)
    {
        _dialogService = dialogService;
        _teacherApiService = teacherApiService;
        _courseApiService = courseApiService;
        _educationalGroupApiService = educationalGroupApiService;

        InitializeComponent();

        _ = LoadDataAsync();
    }

    private static DataGridView CreateStyledGrid()
    {
        var grid = new DataGridView
        {
            BackgroundColor = AppTheme.SurfaceCard,
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

        grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceCardHover;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBodyBold;
        grid.ColumnHeadersHeight = 34;

        grid.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        grid.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
        grid.DefaultCellStyle.SelectionForeColor = AppTheme.TextOnAccent;
        grid.DefaultCellStyle.Font = AppTheme.FontBody;
        grid.RowTemplate.Height = 32;

        return grid;
    }

    private async Task SaveCourseAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtCourseName.Value) || string.IsNullOrWhiteSpace(_txtGradeLevel.Value))
        {
            _dialogService.ShowError("الرجاء إدخال اسم الكورس والصف الدراسي", "تنبيه");
            return;
        }

        try
        {
            var payload = new
            {
                name = _txtCourseName.Value.Trim(),
                gradeLevel = _txtGradeLevel.Value.Trim(),
                subject = _txtSubject.Value.Trim(),
                description = "تم الإنشاء عبر واجهة ويندوز فورمز"
            };

            var success = await _courseApiService.CreateCourseAsync(payload);
            if (success)
            {
                _dialogService.ShowInfo("تم إنشاء الكورس بنجاح", "نجاح");
                _txtCourseName.Value = string.Empty;
                _txtGradeLevel.Value = string.Empty;
                _txtSubject.Value = string.Empty;
                await LoadDataAsync();
            }
            else
            {
                _dialogService.ShowError("فشل الحفظ. يرجى المحاولة مرة أخرى.", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ في الاتصال: {ex.Message}", "خطأ");
        }
    }

    private async Task SaveGroupAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtGroupName.Value) || _cmbCourses.SelectedValue == null || _cmbTeachers.SelectedValue == null)
        {
            _dialogService.ShowError("الرجاء إدخال اسم المجموعة واختيار الكورس والمعلم", "تنبيه");
            return;
        }

        if (!int.TryParse(_txtMaxCapacity.Value, out int maxCapacity) || !decimal.TryParse(_txtMonthlyFee.Value, out decimal monthlyFee))
        {
            _dialogService.ShowError("الرجاء إدخال أرقام صحيحة للسعة والاشتراك", "تنبيه");
            return;
        }

        try
        {
            var payload = new
            {
                courseId = (Guid)_cmbCourses.SelectedValue,
                teacherId = (Guid)_cmbTeachers.SelectedValue,
                name = _txtGroupName.Value.Trim(),
                maxCapacity = maxCapacity,
                monthlyFee = monthlyFee,
                scheduleDescription = _txtSchedule.Value.Trim(),
                status = 0
            };

            var success = await _educationalGroupApiService.CreateGroupAsync(payload);
            if (success)
            {
                _dialogService.ShowInfo("تم إنشاء المجموعة التعليمية بنجاح", "نجاح");
                _txtGroupName.Value = string.Empty;
                _txtMaxCapacity.Value = string.Empty;
                _txtMonthlyFee.Value = string.Empty;
                _txtSchedule.Value = string.Empty;
                await LoadDataAsync();
            }
            else
            {
                _dialogService.ShowError("فشل الحفظ. يرجى المحاولة مرة أخرى.", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ في الاتصال: {ex.Message}", "خطأ");
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var courses = await _courseApiService.GetAllCoursesAsync();
            if (courses != null)
            {
                _gridCourses.DataSource = courses;
                _cmbCourses.DataSource = courses.ToList();
                _cmbCourses.DisplayMember = "Name";
                _cmbCourses.ValueMember = "Id";
            }

            var teachers = await _teacherApiService.GetAllTeachersAsync();
            var teacherItems = teachers.Select(t => new { t.Id, FullName = $"{t.FirstName} {t.LastName}".Trim() }).ToList();
            _cmbTeachers.DataSource = teacherItems;
            _cmbTeachers.DisplayMember = "FullName";
            _cmbTeachers.ValueMember = "Id";

            var groups = await _educationalGroupApiService.GetAllGroupsAsync();
            _gridGroups.DataSource = groups ?? new List<EducationalGroupDto>();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ أثناء جلب البيانات: {ex.Message}", "خطأ");
        }
    }
}
