using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class StudentsView : UserControl
{
    private readonly IStudentApiService _studentApiService;
    private readonly IDialogService _dialogService;
    private readonly IEducationalGroupApiService _educationalGroupApiService;
    private DataGridView _grid = null!;
    private TextBox _searchBox = null!;
    private AppButton _btnSearch = null!;
    private AppButton _btnAddStudent = null!;
    private AppButton _btnEnrollGroup = null!;
    private AppButton _btnRefresh = null!;
    private Label _statusLabel = null!;

    private int _currentPage = 1;
    private int _totalPages = 1;
    private AppButton _btnNext = null!;
    private AppButton _btnPrev = null!;
    private AppButton _btnGoTo = null!;
    private Label _lblPageInfo = null!;
    private TextBox _txtGoToPage = null!;

    public StudentsView(IStudentApiService studentApiService, IDialogService dialogService, IEducationalGroupApiService educationalGroupApiService)
    {
        _studentApiService = studentApiService;
        _dialogService = dialogService;
        _educationalGroupApiService = educationalGroupApiService;

        InitializeComponent();

        _ = LoadStudentsAsync();
    }

    private void ConfigureColumns()
    {
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", Visible = false });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "SerialNumber", FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "كود الطالب", DataPropertyName = "StudentCode", FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", Name = "FullName", DataPropertyName = "FullName", FillWeight = 70 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الهاتف", DataPropertyName = "PhoneNumber", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "هاتف ولي الأمر", DataPropertyName = "ParentPhoneNumber", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الصف الدراسي", DataPropertyName = "GradeLevel", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحالة", DataPropertyName = "StatusText", FillWeight = 30 });
    }

    private async Task LoadStudentsAsync(string? query = null)
    {
        try
        {
            _statusLabel.Text = "جاري التحميل...";
            var paged = await _studentApiService.GetPagedStudentsAsync(_currentPage, 20, query);
            
            if (paged != null)
            {
                var list = new List<EducationCenterSystem.Presentation.WinForms.Models.ViewModels.StudentGridItemViewModel>();
                if (paged?.Items != null)
                {
                    int index = 1;
                    foreach (var s in paged.Items)
                    {
                        list.Add(new EducationCenterSystem.Presentation.WinForms.Models.ViewModels.StudentGridItemViewModel
                        {
                            Id = s.Id,
                            SerialNumber = index++,
                            StudentCode = s.StudentCode,
                            FullName = $"{s.FirstName} {s.LastName}",
                            PhoneNumber = s.PhoneNumber,
                            ParentPhoneNumber = s.ParentPhoneNumber,
                            GradeLevel = s.GradeLevel,
                            StatusText = s.Status == 0 ? "نشط" : "غير نشط"
                        });
                    }
                    _totalPages = paged.TotalPages;
                    _currentPage = paged.PageNumber;
                    _lblPageInfo.Text = $"صفحة {_currentPage} من {Math.Max(1, _totalPages)}";
                    _btnPrev.Enabled = _currentPage > 1;
                    _btnNext.Enabled = _currentPage < _totalPages;
                }
                _grid.DataSource = list;
                _statusLabel.Text = $"تم تحميل {list.Count} طالب";
            }
            else
            {
                _statusLabel.Text = "فشل جلب البيانات من الخادم";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "خطأ في الاتصال";
            _dialogService.ShowError($"خطأ أثناء جلب الطلاب: {ex.Message}", "خطأ");
        }
    }

    private void OpenAddStudentDialog()
    {
        using var form = new Form
        {
            Text = "إضافة طالب جديد",
            Size = new Size(450, 480),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            RightToLeft = RightToLeft.Yes,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var firstNameField = new FormField { LabelText = "الاسم الأول *", Dock = DockStyle.Top };
        var lastNameField = new FormField { LabelText = "الاسم الأخير *", Dock = DockStyle.Top };
        var phoneField = new FormField { LabelText = "رقم الهاتف *", Dock = DockStyle.Top };
        var parentPhoneField = new FormField { LabelText = "رقم ولي الأمر *", Dock = DockStyle.Top };
        var gradeField = new FormField { LabelText = "الصف الدراسي *", Dock = DockStyle.Top };

        var btnSave = new AppButton
        {
            Text = "حفظ البيانات",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Bottom,
            Height = 40
        };

        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(firstNameField.Value) || string.IsNullOrWhiteSpace(lastNameField.Value))
            {
                _dialogService.ShowError("الرجاء إدخال الاسم الأول والأخير", "تنبيه");
                return;
            }

            var payload = new StudentModel
            {
                FirstName = firstNameField.Value.Trim(),
                LastName = lastNameField.Value.Trim(),
                PhoneNumber = phoneField.Value.Trim(),
                ParentPhoneNumber = parentPhoneField.Value.Trim(),
                GradeLevel = gradeField.Value.Trim(),
                DateOfBirth = DateTime.UtcNow.AddYears(-15),
                Gender = EducationCenterSystem.Presentation.WinForms.Models.Enums.Gender.Male
            };

            try
            {
                var success = await _studentApiService.CreateStudentAsync(payload);
                if (success)
                {
                    _dialogService.ShowInfo("تم تسجيل الطالب بنجاح", "نجاح");
                    form.Close();
                    await LoadStudentsAsync();
                }
                else
                {
                    _dialogService.ShowError("فشل التسجيل، يرجى مراجعة البيانات أو الخادم.", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ في الاتصال: {ex.Message}", "خطأ");
            }
        };

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        container.Controls.Add(gradeField);
        container.Controls.Add(parentPhoneField);
        container.Controls.Add(phoneField);
        container.Controls.Add(lastNameField);
        container.Controls.Add(firstNameField);
        container.Controls.Add(btnSave);

        form.Controls.Add(container);
        form.ShowDialog(this);
    }

    private async Task OpenEnrollStudentDialogAsync()
    {
        if (_grid.SelectedRows.Count == 0)
        {
            _dialogService.ShowError("الرجاء تحديد طالب من القائمة أولاً", "تنبيه");
            return;
        }

        var selectedRow = _grid.SelectedRows[0];
        if (selectedRow.Cells["Id"].Value is not Guid studentId)
        {
            _dialogService.ShowError("لم يتم العثور على معرف الطالب.", "خطأ");
            return;
        }
        var studentName = selectedRow.Cells["FullName"].Value?.ToString() ?? "الطالب";

        // Fetch groups
        List<EducationalGroupDto>? groups = null;
        try
        {
            groups = await _educationalGroupApiService.GetAllGroupsAsync();
        }
        catch(Exception ex)
        {
            _dialogService.ShowError($"خطأ أثناء الاتصال: {ex.Message}", "خطأ");
            return;
        }

        if (groups == null || groups.Count == 0)
        {
            _dialogService.ShowInfo("لا يوجد مجموعات متاحة للتسجيل.", "معلومة");
            return;
        }

        using var form = new Form
        {
            Text = $"تسجيل الطالب: {studentName}",
            Size = new Size(400, 250),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            RightToLeft = RightToLeft.Yes,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var pnlContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        var lblGroup = new Label { Text = "اختر المجموعة:", Dock = DockStyle.Top, Height = 25, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption };
        var cmbGroups = new ComboBox
        {
            Dock = DockStyle.Top,
            Height = 35,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = groups,
            DisplayMember = "Name",
            ValueMember = "Id",
            BackColor = AppTheme.SurfaceCard,
            ForeColor = AppTheme.TextPrimary,
            Font = AppTheme.FontBody
        };

        var btnSave = new AppButton
        {
            Text = "حفظ وإضافة للمجموعة",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Bottom,
            Height = 40
        };

        btnSave.Click += async (s, e) =>
        {
            if (cmbGroups.SelectedValue == null) return;
            var groupId = (Guid)cmbGroups.SelectedValue;
            
            try
            {
                var success = await _educationalGroupApiService.EnrollStudentAsync(groupId, studentId);
                if (success)
                {
                    _dialogService.ShowInfo("تم التسجيل بنجاح!", "نجاح");
                    form.Close();
                }
                else
                {
                    _dialogService.ShowError("فشل التسجيل. تأكد من البيانات أو حاول مرة أخرى.", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ أثناء الاتصال: {ex.Message}", "خطأ");
            }
        };

        pnlContainer.Controls.Add(cmbGroups);
        pnlContainer.Controls.Add(lblGroup);
        pnlContainer.Controls.Add(btnSave);

        form.Controls.Add(pnlContainer);
        form.ShowDialog(this);
    }
}
