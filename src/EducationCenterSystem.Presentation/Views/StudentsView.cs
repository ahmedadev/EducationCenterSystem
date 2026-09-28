using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class StudentsView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly DataGridView _grid;
    private readonly TextBox _searchBox;
    private readonly AppButton _btnSearch;
    private readonly AppButton _btnAddStudent;
    private readonly AppButton _btnRefresh;
    private readonly Label _statusLabel;

    public StudentsView(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        // Top Header & Action Bar
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var titleLabel = new Label
        {
            Text = "إدارة الطلاب",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(16, 10)
        };

        var subtitleLabel = new Label
        {
            Text = "عرض بيانات الطلاب المسجلين والبحث وتعديل الحسابات",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(16, 46)
        };

        _btnAddStudent = new AppButton
        {
            Text = "+ إضافة طالب",
            Variant = ButtonVariant.Primary,
            Width = 130,
            Height = 36,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(16, 24)
        };
        _btnAddStudent.Click += (s, e) => OpenAddStudentDialog();

        _btnRefresh = new AppButton
        {
            Text = "تحديث",
            Variant = ButtonVariant.Secondary,
            Width = 90,
            Height = 36,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(155, 24)
        };
        _btnRefresh.Click += async (s, e) => await LoadStudentsAsync();

        topPanel.Controls.Add(titleLabel);
        topPanel.Controls.Add(subtitleLabel);
        topPanel.Controls.Add(_btnAddStudent);
        topPanel.Controls.Add(_btnRefresh);

        // Search Bar Panel
        var searchPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 55,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(16, 10, 16, 10)
        };

        _searchBox = new TextBox
        {
            Width = 260,
            Height = 32,
            Font = AppTheme.FontBody,
            BackColor = AppTheme.SurfaceCard,
            ForeColor = AppTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(16, 12)
        };

        _btnSearch = new AppButton
        {
            Text = "بحث",
            Variant = ButtonVariant.Secondary,
            Width = 80,
            Height = 32,
            Location = new Point(285, 11)
        };
        _btnSearch.Click += async (s, e) => await LoadStudentsAsync(_searchBox.Text);

        _statusLabel = new Label
        {
            Text = "جاهز",
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.FontCaption,
            AutoSize = true,
            Location = new Point(380, 18)
        };

        searchPanel.Controls.Add(_searchBox);
        searchPanel.Controls.Add(_btnSearch);
        searchPanel.Controls.Add(_statusLabel);

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
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        _grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBodyBold;
        _grid.ColumnHeadersHeight = 40;

        _grid.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        _grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        _grid.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
        _grid.DefaultCellStyle.SelectionForeColor = AppTheme.TextOnAccent;
        _grid.DefaultCellStyle.Font = AppTheme.FontBody;
        _grid.RowTemplate.Height = 36;

        ConfigureColumns();

        Controls.Add(_grid);
        Controls.Add(searchPanel);
        Controls.Add(topPanel);

        _ = LoadStudentsAsync();
    }

    private void ConfigureColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "SerialNumber", FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "كود الطالب", DataPropertyName = "StudentCode", FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 70 });
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
            var url = string.IsNullOrWhiteSpace(query) 
                ? "api/students?page=1&pageSize=50" 
                : $"api/students?page=1&pageSize=50&searchTerm={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var paged = await response.Content.ReadFromJsonAsync<PagedResultModel<StudentModel>>();
                var list = new List<object>();
                if (paged?.Items != null)
                {
                    int index = 1;
                    foreach (var s in paged.Items)
                    {
                        list.Add(new
                        {
                            SerialNumber = index++,
                            s.StudentCode,
                            FullName = $"{s.FirstName} {s.LastName}",
                            s.PhoneNumber,
                            s.ParentPhoneNumber,
                            s.GradeLevel,
                            StatusText = s.Status == 0 ? "نشط" : "غير نشط"
                        });
                    }
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

            var payload = new
            {
                firstName = firstNameField.Value.Trim(),
                lastName = lastNameField.Value.Trim(),
                phoneNumber = phoneField.Value.Trim(),
                parentPhoneNumber = parentPhoneField.Value.Trim(),
                gradeLevel = gradeField.Value.Trim(),
                dateOfBirth = DateTime.UtcNow.AddYears(-15),
                gender = 1
            };

            try
            {
                var res = await _httpClient.PostAsJsonAsync("api/students", payload);
                if (res.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تسجيل الطالب بنجاح", "نجاح");
                    form.Close();
                    await LoadStudentsAsync();
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    _dialogService.ShowError($"فشل التسجيل: {err}", "خطأ");
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
}
