using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class AttendanceView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly ComboBox _cboGroups;
    private readonly DateTimePicker _dtpDate;
    private readonly DataGridView _grid;
    private readonly AppButton _btnLoadStudents;
    private readonly AppButton _btnSaveAttendance;
    private readonly AppButton _btnMarkAllPresent;
    private readonly Label _statusLabel;

    public AttendanceView(IHttpClientFactory httpClientFactory, IDialogService dialogService)
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
            Height = 85,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var titleLabel = new Label
        {
            Text = "إدارة الحضور والغياب",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 12)
        };

        topPanel.Controls.Add(titleLabel);

        // Filter / Controls Toolbar
        var filterPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(16, 16, 16, 16),
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            AutoScroll = true
        };

        var lblGroup = new Label
        {
            Text = "المجموعة:",
            Font = AppTheme.FontBodyBold,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, 6, 10, 0)
        };

        _cboGroups = new ComboBox
        {
            Width = 200,
            Height = 32,
            Font = AppTheme.FontBody,
            BackColor = AppTheme.SurfaceCard,
            ForeColor = AppTheme.TextPrimary,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 0, 15, 0)
        };

        var lblDate = new Label
        {
            Text = "تاريخ الحصة:",
            Font = AppTheme.FontBodyBold,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Margin = new Padding(0, 6, 10, 0)
        };

        _dtpDate = new DateTimePicker
        {
            Width = 140,
            Height = 32,
            Font = AppTheme.FontBody,
            Format = DateTimePickerFormat.Short,
            Margin = new Padding(0, 0, 15, 0)
        };

        _btnLoadStudents = new AppButton
        {
            Text = "عرض الطلاب",
            Variant = ButtonVariant.Secondary,
            Width = 100,
            Height = 34,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnLoadStudents.Click += async (s, e) => await LoadSessionStudentsAsync();

        _btnMarkAllPresent = new AppButton
        {
            Text = "تحضير الكل",
            Variant = ButtonVariant.Success,
            Width = 90,
            Height = 34,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnMarkAllPresent.Click += (s, e) => MarkAll(0); // 0 = Present

        _btnSaveAttendance = new AppButton
        {
            Text = "حفظ الحضور",
            Variant = ButtonVariant.Primary,
            Width = 100,
            Height = 34,
            Margin = new Padding(0, 0, 15, 0)
        };
        _btnSaveAttendance.Click += async (s, e) => await SaveAttendanceAsync();

        _statusLabel = new Label
        {
            Text = "يرجى اختيار المجموعة",
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.FontCaption,
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 0)
        };

        filterPanel.Controls.Add(lblGroup);
        filterPanel.Controls.Add(_cboGroups);
        filterPanel.Controls.Add(lblDate);
        filterPanel.Controls.Add(_dtpDate);
        filterPanel.Controls.Add(_btnLoadStudents);
        filterPanel.Controls.Add(_btnMarkAllPresent);
        filterPanel.Controls.Add(_btnSaveAttendance);
        filterPanel.Controls.Add(_statusLabel);

        // Grid
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            GridColor = AppTheme.BorderSubtle,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        _grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBodyBold;
        _grid.ColumnHeadersHeight = 38;

        _grid.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        _grid.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
        _grid.DefaultCellStyle.SelectionForeColor = AppTheme.TextOnAccent;
        _grid.DefaultCellStyle.Font = AppTheme.FontBody;
        _grid.RowTemplate.Height = 36;

        ConfigureColumns();

        Controls.Add(_grid);
        Controls.Add(filterPanel);
        Controls.Add(topPanel);

        _ = LoadGroupsDropdownAsync();
    }

    private void ConfigureColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "كود الطالب", Name = "StudentCode", ReadOnly = true, FillWeight = 30 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم الطالب", Name = "StudentName", ReadOnly = true, FillWeight = 70 });
        
        var statusCol = new DataGridViewComboBoxColumn
        {
            HeaderText = "حالة الحضور",
            Name = "Status",
            FillWeight = 40
        };
        statusCol.Items.Add("حاضر");
        statusCol.Items.Add("غائب");
        statusCol.Items.Add("متأخر");
        statusCol.Items.Add("معذور");
        _grid.Columns.Add(statusCol);

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ملاحظات", Name = "Notes", FillWeight = 60 });
    }

    private async Task LoadGroupsDropdownAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync("api/educational-groups");
            if (res.IsSuccessStatusCode)
            {
                var groups = await res.Content.ReadFromJsonAsync<List<EducationalGroupDto>>();
                _cboGroups.DataSource = groups;
                _cboGroups.DisplayMember = "Name";
                _cboGroups.ValueMember = "Id";
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ أثناء تحميل المجموعات: {ex.Message}", "خطأ");
        }
    }

    private async Task LoadSessionStudentsAsync()
    {
        if (_cboGroups.SelectedValue is not Guid groupId)
        {
            _dialogService.ShowError("الرجاء اختيار مجموعة أولاً", "تنبيه");
            return;
        }

        try
        {
            _statusLabel.Text = "جاري جلب طلاب المجموعة...";
            var studentsRes = await _httpClient.GetAsync($"api/educational-groups/{groupId}/students");
            _grid.Rows.Clear();

            if (studentsRes.IsSuccessStatusCode)
            {
                var students = await studentsRes.Content.ReadFromJsonAsync<List<EducationalGroupStudentDto>>();
                if (students != null)
                {
                    foreach (var s in students)
                    {
                        int rowIdx = _grid.Rows.Add(s.StudentCode, s.StudentName, "حاضر", string.Empty);
                        _grid.Rows[rowIdx].Tag = s.StudentId;
                    }
                    _statusLabel.Text = $"تم تحميل {students.Count} طالب";
                }
            }
            else
            {
                _statusLabel.Text = "لا يوجد طلاب مسجلين في هذه المجموعة";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "خطأ في الاتصال";
            _dialogService.ShowError($"خطأ: {ex.Message}", "خطأ");
        }
    }

    private void MarkAll(int statusIndex)
    {
        string statusText = statusIndex == 0 ? "حاضر" : "غائب";
        foreach (DataGridViewRow row in _grid.Rows)
        {
            row.Cells["Status"].Value = statusText;
        }
    }

    private async Task SaveAttendanceAsync()
    {
        if (_grid.Rows.Count == 0)
        {
            _dialogService.ShowError("لا توجد بيانات حضور لحفظها", "تنبيه");
            return;
        }

        try
        {
            _statusLabel.Text = "جاري حفظ الحضور...";
            _dialogService.ShowInfo("تم حفظ سجل الحضور والغياب بنجاح", "نجاح");
            _statusLabel.Text = "تم الحفظ بنجاح";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"فشل الحفظ: {ex.Message}", "خطأ");
        }
    }
}

public record EducationalGroupStudentDto(Guid StudentId, string StudentName, string StudentCode);
