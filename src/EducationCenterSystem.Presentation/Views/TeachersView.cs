using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class TeachersView : UserControl
{
    private readonly ITeacherApiService _teacherApiService;
    private readonly IDialogService _dialogService;
    private DataGridView _grid = null!;
    private TextBox _searchBox = null!;
    private AppButton _btnSearch = null!;
    private AppButton _btnAddTeacher = null!;
    private AppButton _btnRefresh = null!;
    private Label _statusLabel = null!;

    private int _currentPage = 1;
    private int _totalPages = 1;
    private AppButton _btnNext = null!;
    private AppButton _btnPrev = null!;
    private AppButton _btnGoTo = null!;
    private Label _lblPageInfo = null!;
    private TextBox _txtGoToPage = null!;
    public TeachersView(ITeacherApiService teacherApiService, IDialogService dialogService)
    {
        _teacherApiService = teacherApiService;
        _dialogService = dialogService;

        InitializeComponent();

        _ = LoadTeachersAsync();
    }

    private void ConfigureColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "SerialNumber", FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "كود المعلم", DataPropertyName = "TeacherCode", FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 70 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المادة / التخصص", DataPropertyName = "Specialization", FillWeight = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الهاتف", DataPropertyName = "PhoneNumber", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "البريد الإلكتروني", DataPropertyName = "Email", FillWeight = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحالة", DataPropertyName = "StatusText", FillWeight = 30 });
    }

    private async Task LoadTeachersAsync(string? query = null)
    {
        try
        {
            _statusLabel.Text = "جاري التحميل...";
            var paged = await _teacherApiService.GetPagedTeachersAsync(_currentPage, 20, query);
            
            if (paged != null)
            {
                var list = new List<EducationCenterSystem.Presentation.WinForms.Models.ViewModels.TeacherGridItemViewModel>();
                if (paged?.Items != null)
                {
                    int index = 1;
                    foreach (var t in paged.Items)
                    {
                        list.Add(new EducationCenterSystem.Presentation.WinForms.Models.ViewModels.TeacherGridItemViewModel
                        {
                            SerialNumber = index++,
                            TeacherCode = t.TeacherCode,
                            FullName = $"{t.FirstName} {t.SecondName} {t.ThirdName} {t.LastName}".Replace("  ", " ").Trim(),
                            Specialization = t.Subject,
                            PhoneNumber = t.PhoneNumber,
                            Email = t.Email,
                            StatusText = t.Status == 0 ? "نشط" : "غير نشط"
                        });
                    }
                    _totalPages = paged.TotalPages;
                    _currentPage = paged.PageNumber;
                    _lblPageInfo.Text = $"صفحة {_currentPage} من {Math.Max(1, _totalPages)}";
                    _btnPrev.Enabled = _currentPage > 1;
                    _btnNext.Enabled = _currentPage < _totalPages;
                }
                _grid.DataSource = list;
                _statusLabel.Text = $"تم تحميل {list.Count} معلم";
            }
            else
            {
                _statusLabel.Text = "فشل جلب البيانات من الخادم";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "خطأ في الاتصال";
            _dialogService.ShowError($"خطأ أثناء جلب المعلمين: {ex.Message}", "خطأ");
        }
    }

    private void OpenAddTeacherDialog()
    {
        using var form = new Form
        {
            Text = "إضافة معلم جديد",
            Size = new Size(500, 750),
            StartPosition = FormStartPosition.CenterParent,
            BackColor = AppTheme.BackgroundDark,
            ForeColor = AppTheme.TextPrimary,
            RightToLeft = RightToLeft.Yes,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var firstNameField = new FormField { LabelText = "الاسم الأول *", Dock = DockStyle.Top };
        var secondNameField = new FormField { LabelText = "الاسم الثاني *", Dock = DockStyle.Top };
        var thirdNameField = new FormField { LabelText = "الاسم الثالث *", Dock = DockStyle.Top };
        var lastNameField = new FormField { LabelText = "الاسم الأخير *", Dock = DockStyle.Top };
        var emailField = new FormField { LabelText = "البريد الإلكتروني", Dock = DockStyle.Top };
        var phoneField = new FormField { LabelText = "رقم الهاتف *", Dock = DockStyle.Top };
        
        var dateOfBirthPanel = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
        var dateOfBirthLabel = new Label { Text = "تاريخ الميلاد", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
        var dateOfBirthPicker = new DateTimePicker { Dock = DockStyle.Bottom, Height = 32, Format = DateTimePickerFormat.Short, RightToLeftLayout = true };
        dateOfBirthPanel.Controls.Add(dateOfBirthPicker);
        dateOfBirthPanel.Controls.Add(dateOfBirthLabel);

        var nationalIdField = new FormField { LabelText = "الرقم القومي", Dock = DockStyle.Top };
        var teacherCodeField = new FormField { LabelText = "كود المعلم *", Dock = DockStyle.Top };
        var specField = new FormField { LabelText = "المادة / التخصص *", Dock = DockStyle.Top };
        var qualField = new FormField { LabelText = "المؤهل", Dock = DockStyle.Top };
        
        var genderPanel = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
        var genderLabel = new Label { Text = "الجنس", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
        var genderCombo = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary };
        genderCombo.Items.Add("ذكر");
        genderCombo.Items.Add("أنثى");
        genderCombo.SelectedIndex = 0;
        genderPanel.Controls.Add(genderCombo);
        genderPanel.Controls.Add(genderLabel);

        var addressField = new FormField { LabelText = "العنوان", Dock = DockStyle.Top };
        var notesField = new FormField { LabelText = "ملاحظات", Dock = DockStyle.Top };

        var btnSave = new AppButton
        {
            Text = "حفظ البيانات",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Fill,
            Height = 40
        };

        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(firstNameField.Value) || 
                string.IsNullOrWhiteSpace(secondNameField.Value) || 
                string.IsNullOrWhiteSpace(thirdNameField.Value) || 
                string.IsNullOrWhiteSpace(lastNameField.Value) || 
                string.IsNullOrWhiteSpace(teacherCodeField.Value) ||
                string.IsNullOrWhiteSpace(specField.Value) ||
                string.IsNullOrWhiteSpace(phoneField.Value))
            {
                _dialogService.ShowError("الرجاء إدخال الحقول المطلوبة الأساسية (*)", "تنبيه");
                return;
            }

            var payload = new TeacherModel
            {
                FirstName = firstNameField.Value.Trim(),
                SecondName = secondNameField.Value.Trim(),
                ThirdName = thirdNameField.Value.Trim(),
                LastName = lastNameField.Value.Trim(),
                Email = emailField.Value.Trim(),
                PhoneNumber = phoneField.Value.Trim(),
                DateOfBirth = dateOfBirthPicker.Value,
                NationalId = string.IsNullOrWhiteSpace(nationalIdField.Value) ? null : nationalIdField.Value.Trim(),
                TeacherCode = teacherCodeField.Value.Trim(),
                Subject = specField.Value.Trim(),
                Qualification = string.IsNullOrWhiteSpace(qualField.Value) ? null : qualField.Value.Trim(),
                Gender = genderCombo.SelectedIndex == 0 ? EducationCenterSystem.Presentation.WinForms.Models.Enums.Gender.Male : EducationCenterSystem.Presentation.WinForms.Models.Enums.Gender.Female,
                Address = string.IsNullOrWhiteSpace(addressField.Value) ? null : addressField.Value.Trim(),
                Notes = string.IsNullOrWhiteSpace(notesField.Value) ? null : notesField.Value.Trim()
            };

            try
            {
                var success = await _teacherApiService.CreateTeacherAsync(payload);
                if (success)
                {
                    _dialogService.ShowInfo("تم تسجيل المعلم بنجاح", "نجاح");
                    form.Close();
                    await LoadTeachersAsync();
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

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };
        container.Controls.Add(notesField);
        container.Controls.Add(addressField);
        container.Controls.Add(genderPanel);
        container.Controls.Add(qualField);
        container.Controls.Add(specField);
        container.Controls.Add(teacherCodeField);
        container.Controls.Add(nationalIdField);
        container.Controls.Add(dateOfBirthPanel);
        container.Controls.Add(phoneField);
        container.Controls.Add(emailField);
        container.Controls.Add(lastNameField);
        container.Controls.Add(thirdNameField);
        container.Controls.Add(secondNameField);
        container.Controls.Add(firstNameField);
        
        var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(20) };
        bottomPanel.Controls.Add(btnSave);

        form.Controls.Add(container);
        form.Controls.Add(bottomPanel);
        form.ShowDialog(this);
    }
}
