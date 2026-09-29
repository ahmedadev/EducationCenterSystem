using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class AttendanceView : UserControl
{
    private readonly IEducationalGroupApiService _educationalGroupApiService;
    private readonly IAttendanceApiService _attendanceApiService;
    private readonly IDialogService _dialogService;
    private ComboBox _cboGroups = null!;
    private DateTimePicker _dtpDate = null!;
    private DataGridView _grid = null!;
    private AppButton _btnLoadStudents = null!;
    private AppButton _btnSaveAttendance = null!;
    private AppButton _btnMarkAllPresent = null!;
    private Label _statusLabel = null!;

    public AttendanceView(IDialogService dialogService,
                          IEducationalGroupApiService educationalGroupApiService,
                          IAttendanceApiService attendanceApiService)
    {
        _dialogService = dialogService;
        _educationalGroupApiService = educationalGroupApiService;
        _attendanceApiService = attendanceApiService;

        InitializeComponent();
        _grid.ApplyModernTheme();


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
            var groups = await _educationalGroupApiService.GetAllGroupsAsync();
            if (groups != null)
            {
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
            var students = await _educationalGroupApiService.GetGroupStudentsAsync(groupId);
            _grid.Rows.Clear();

            if (students != null && students.Count > 0)
            {
                foreach (var s in students)
                {
                    int rowIdx = _grid.Rows.Add(s.StudentCode, s.StudentName, "حاضر", string.Empty);
                    _grid.Rows[rowIdx].Tag = s.StudentId;
                }
                _statusLabel.Text = $"تم تحميل {students.Count} طالب";
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
        if (_grid.Rows.Count == 0 || _cboGroups.SelectedValue == null)
        {
            _dialogService.ShowError("الرجاء اختيار مجموعة وعرض الطلاب أولاً", "تنبيه");
            return;
        }

        try
        {
            _statusLabel.Text = "جاري حفظ الحضور...";
            
            var groupId = (Guid)_cboGroups.SelectedValue;
            var sessionDate = _dtpDate.Value.Date;
            
            var attendanceList = new List<object>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Tag is Guid studentId)
                {
                    var statusText = row.Cells["Status"].Value?.ToString();
                    int statusEnum = statusText switch {
                        "حاضر" => 1,
                        "غائب" => 2,
                        "متأخر" => 3,
                        "معذور" => 4,
                        _ => 1
                    };
                    var notes = row.Cells["Notes"].Value?.ToString() ?? "";

                    attendanceList.Add(new {
                        studentId,
                        status = statusEnum,
                        notes
                    });
                }
            }

            var payload = new {
                groupId,
                sessionDate,
                records = attendanceList
            };

            var success = await _attendanceApiService.SubmitBatchAttendanceAsync(payload);
            
            if (success)
            {
                _dialogService.ShowInfo("تم حفظ سجل الحضور والغياب بنجاح", "نجاح");
                _statusLabel.Text = "تم الحفظ بنجاح";
            }
            else
            {
                _dialogService.ShowError("فشل الحفظ. يرجى المحاولة مرة أخرى.", "خطأ");
                _statusLabel.Text = "فشل الحفظ";
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"فشل الحفظ: {ex.Message}", "خطأ");
            _statusLabel.Text = "فشل الحفظ";
        }
    }
}

