using System.Net.Http.Json;
using System.Linq;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class CoursesAndGroupsView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ITeacherApiService _teacherApiService;

    private readonly DataGridView _gridCourses;
    private readonly DataGridView _gridGroups;
    private readonly FormField _txtCourseName;
    private readonly FormField _txtGradeLevel;
    private readonly FormField _txtSubject;
    private readonly AppButton _btnSaveCourse;
    private readonly AppButton _btnRefresh;

    private readonly ComboBox _cmbCourses;
    private readonly ComboBox _cmbTeachers;
    private readonly FormField _txtGroupName;
    private readonly FormField _txtMaxCapacity;
    private readonly FormField _txtMonthlyFee;
    private readonly FormField _txtSchedule;
    private readonly AppButton _btnSaveGroup;

    public CoursesAndGroupsView(IHttpClientFactory httpClientFactory, IDialogService dialogService, ITeacherApiService teacherApiService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
        _teacherApiService = teacherApiService;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        // Top Header
        var topPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12),
            ColumnCount = 2,
            RowCount = 1,
            RightToLeft = RightToLeft.Yes
        };
        topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var titleContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        var titleLabel = new Label
        {
            Text = "إدارة المواد والمجموعات التعليمية",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        titleContainer.Controls.Add(titleLabel);

        var actionContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 10, 0, 0),
            AutoSize = true
        };

        _btnRefresh = new AppButton
        {
            Text = "تحديث الكل",
            Variant = ButtonVariant.Secondary,
            Width = 110,
            Height = 40,
            Margin = new Padding(0)
        };
        _btnRefresh.Click += async (s, e) => await LoadDataAsync();

        actionContainer.Controls.Add(_btnRefresh);

        topPanel.Controls.Add(titleContainer, 0, 0);
        topPanel.Controls.Add(actionContainer, 1, 0);

        // Main Layout (Split: Right is Forms, Left is Tables)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 350,
            BackColor = AppTheme.BorderSubtle,
            FixedPanel = FixedPanel.Panel1
        };

        // Panel 1: Forms Panel
        var pnlForm = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16),
            AutoScroll = true
        };

        // --- ADD GROUP SECTION ---
        var pnlGroupForm = new Panel { Dock = DockStyle.Top, Height = 480, Padding = new Padding(0, 20, 0, 0) };
        var groupTitle = new Label { Text = "إضافة مجموعة تعليمية جديدة", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, Dock = DockStyle.Top, Height = 35 };

        var pnlCmbCourse = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
        var lblCourse = new Label { Text = "الكورس المرتبط *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
        _cmbCourses = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
        pnlCmbCourse.Controls.Add(_cmbCourses);
        pnlCmbCourse.Controls.Add(lblCourse);

        var pnlCmbTeacher = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
        var lblTeacher = new Label { Text = "معلم المجموعة *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
        _cmbTeachers = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
        pnlCmbTeacher.Controls.Add(_cmbTeachers);
        pnlCmbTeacher.Controls.Add(lblTeacher);

        _txtGroupName = new FormField { LabelText = "اسم المجموعة (مثال: السبت والثلاثاء 5م) *", Dock = DockStyle.Top };
        _txtMaxCapacity = new FormField { LabelText = "الحد الأقصى للطلاب *", Dock = DockStyle.Top };
        _txtMonthlyFee = new FormField { LabelText = "الاشتراك الشهري *", Dock = DockStyle.Top };
        _txtSchedule = new FormField { LabelText = "مواعيد الحضور", Dock = DockStyle.Top };
        
        _btnSaveGroup = new AppButton { Text = "حفظ المجموعة", Variant = ButtonVariant.Primary, Dock = DockStyle.Top, Height = 40 };
        _btnSaveGroup.Click += async (s, e) => await SaveGroupAsync();

        pnlGroupForm.Controls.Add(_btnSaveGroup);
        pnlGroupForm.Controls.Add(_txtSchedule);
        pnlGroupForm.Controls.Add(_txtMonthlyFee);
        pnlGroupForm.Controls.Add(_txtMaxCapacity);
        pnlGroupForm.Controls.Add(_txtGroupName);
        pnlGroupForm.Controls.Add(pnlCmbTeacher);
        pnlGroupForm.Controls.Add(pnlCmbCourse);
        pnlGroupForm.Controls.Add(groupTitle);

        // --- ADD COURSE SECTION ---
        var pnlCourseForm = new Panel { Dock = DockStyle.Top, Height = 280, Padding = new Padding(0, 0, 0, 20) };
        var formTitle = new Label { Text = "إضافة مادة / كورس جديد", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, Dock = DockStyle.Top, Height = 35 };

        _txtCourseName = new FormField { LabelText = "اسم المادة / الكورس *", Dock = DockStyle.Top };
        _txtGradeLevel = new FormField { LabelText = "الصف الدراسي *", Dock = DockStyle.Top };
        _txtSubject = new FormField { LabelText = "المادة العلمية *", Dock = DockStyle.Top };

        _btnSaveCourse = new AppButton { Text = "حفظ الكورس", Variant = ButtonVariant.Primary, Dock = DockStyle.Top, Height = 40 };
        _btnSaveCourse.Click += async (s, e) => await SaveCourseAsync();

        pnlCourseForm.Controls.Add(_btnSaveCourse);
        pnlCourseForm.Controls.Add(_txtSubject);
        pnlCourseForm.Controls.Add(_txtGradeLevel);
        pnlCourseForm.Controls.Add(_txtCourseName);
        pnlCourseForm.Controls.Add(formTitle);

        pnlForm.Controls.Add(pnlGroupForm);
        pnlForm.Controls.Add(pnlCourseForm); 

        split.Panel1.Controls.Add(pnlForm);

        // Panel 2: Grids for Courses and Groups
        var pnlGrids = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(12)
        };

        var lblCourses = new Label
        {
            Text = "المواد الدراسية الحالية",
            Font = AppTheme.FontSubtitle,
            ForeColor = AppTheme.AccentPrimary,
            Dock = DockStyle.Top,
            Height = 28
        };

        _gridCourses = CreateStyledGrid();
        _gridCourses.Dock = DockStyle.Top;
        _gridCourses.Height = 220;
        _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم الكورس", DataPropertyName = "Name", FillWeight = 60 });
        _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الصف الدراسي", DataPropertyName = "GradeLevel", FillWeight = 40 });
        _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المادة", DataPropertyName = "Subject", FillWeight = 40 });

        var lblGroups = new Label
        {
            Text = "المجموعات التعليمية المرتبطة",
            Font = AppTheme.FontSubtitle,
            ForeColor = AppTheme.AccentPrimary,
            Dock = DockStyle.Top,
            Height = 35
        };

        _gridGroups = CreateStyledGrid();
        _gridGroups.Dock = DockStyle.Fill;
        _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم المجموعة", DataPropertyName = "Name", FillWeight = 50 });
        _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المادة", DataPropertyName = "CourseName", FillWeight = 40 });
        _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المعلم", DataPropertyName = "TeacherName", FillWeight = 40 });
        _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "السعة", DataPropertyName = "MaxCapacity", FillWeight = 25 });
        _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاشتراك الشهري", DataPropertyName = "MonthlyFee", FillWeight = 35 });

        pnlGrids.Controls.Add(_gridGroups);
        pnlGrids.Controls.Add(lblGroups);
        pnlGrids.Controls.Add(_gridCourses);
        pnlGrids.Controls.Add(lblCourses);
        split.Panel2.Controls.Add(pnlGrids);

        Controls.Add(split);
        Controls.Add(topPanel);

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

            var res = await _httpClient.PostAsJsonAsync("api/courses", payload);
            if (res.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo("تم إنشاء الكورس بنجاح", "نجاح");
                _txtCourseName.Value = string.Empty;
                _txtGradeLevel.Value = string.Empty;
                _txtSubject.Value = string.Empty;
                await LoadDataAsync();
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                _dialogService.ShowError($"فشل الحفظ: {err}", "خطأ");
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

            var res = await _httpClient.PostAsJsonAsync("api/educational-groups", payload);
            if (res.IsSuccessStatusCode)
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
                var err = await res.Content.ReadAsStringAsync();
                _dialogService.ShowError($"فشل الحفظ: {err}", "خطأ");
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
            var coursesRes = await _httpClient.GetAsync("api/courses");
            if (coursesRes.IsSuccessStatusCode)
            {
                var courses = await coursesRes.Content.ReadFromJsonAsync<List<CourseDto>>();
                if (courses != null)
                {
                    _gridCourses.DataSource = courses;
                    _cmbCourses.DataSource = courses.ToList();
                    _cmbCourses.DisplayMember = "Name";
                    _cmbCourses.ValueMember = "Id";
                }
            }

            var teachers = await _teacherApiService.GetAllTeachersAsync();
            var teacherItems = teachers.Select(t => new { t.Id, FullName = $"{t.FirstName} {t.LastName}".Trim() }).ToList();
            _cmbTeachers.DataSource = teacherItems;
            _cmbTeachers.DisplayMember = "FullName";
            _cmbTeachers.ValueMember = "Id";

            var groupsRes = await _httpClient.GetAsync("api/educational-groups");
            if (groupsRes.IsSuccessStatusCode)
            {
                var groups = await groupsRes.Content.ReadFromJsonAsync<List<EducationalGroupDto>>();
                _gridGroups.DataSource = groups ?? new List<EducationalGroupDto>();
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ أثناء جلب البيانات: {ex.Message}", "خطأ");
        }
    }
}
