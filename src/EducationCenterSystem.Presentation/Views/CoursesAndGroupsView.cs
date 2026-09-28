using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class CoursesAndGroupsView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly DataGridView _gridCourses;
    private readonly DataGridView _gridGroups;
    private readonly FormField _txtCourseName;
    private readonly FormField _txtGradeLevel;
    private readonly FormField _txtSubject;
    private readonly AppButton _btnSaveCourse;
    private readonly AppButton _btnRefresh;

    public CoursesAndGroupsView(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;

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

        // Main Layout (Split: Right is Course Form, Left is Tables)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 350,
            BackColor = AppTheme.BorderSubtle,
            FixedPanel = FixedPanel.Panel1
        };

        // Panel 1: Add Course Panel
        var pnlForm = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16)
        };

        var formTitle = new Label
        {
            Text = "إضافة مادة / كورس جديد",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            Dock = DockStyle.Top,
            Height = 35
        };

        _txtCourseName = new FormField { LabelText = "اسم المادة / الكورس *", Dock = DockStyle.Top };
        _txtGradeLevel = new FormField { LabelText = "الصف الدراسي *", Dock = DockStyle.Top };
        _txtSubject = new FormField { LabelText = "المادة العلمية *", Dock = DockStyle.Top };

        _btnSaveCourse = new AppButton
        {
            Text = "حفظ الكورس",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Top,
            Height = 40
        };
        _btnSaveCourse.Click += async (s, e) => await SaveCourseAsync();

        pnlForm.Controls.Add(_btnSaveCourse);
        pnlForm.Controls.Add(_txtSubject);
        pnlForm.Controls.Add(_txtGradeLevel);
        pnlForm.Controls.Add(_txtCourseName);
        pnlForm.Controls.Add(formTitle);
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

    private async Task LoadDataAsync()
    {
        try
        {
            var coursesRes = await _httpClient.GetAsync("api/courses");
            if (coursesRes.IsSuccessStatusCode)
            {
                var courses = await coursesRes.Content.ReadFromJsonAsync<List<CourseDto>>();
                _gridCourses.DataSource = courses ?? new List<CourseDto>();
            }

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
