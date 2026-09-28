using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class TeachersView : UserControl
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;
    private readonly DataGridView _grid;
    private readonly TextBox _searchBox;
    private readonly AppButton _btnSearch;
    private readonly AppButton _btnAddTeacher;
    private readonly AppButton _btnRefresh;
    private readonly Label _statusLabel;

    public TeachersView(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = AppTheme.SurfaceCard,
            Padding = new Padding(16, 12, 16, 12)
        };

        var titleLabel = new Label
        {
            Text = "إدارة المعلمين",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(16, 10)
        };

        var subtitleLabel = new Label
        {
            Text = "سجل الكادر التعليمي، التخصصات، وأرقام التواصل",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(16, 46)
        };

        _btnAddTeacher = new AppButton
        {
            Text = "+ إضافة معلم",
            Variant = ButtonVariant.Primary,
            Width = 130,
            Height = 36,
            Location = new Point(16, 24)
        };
        _btnAddTeacher.Click += (s, e) => OpenAddTeacherDialog();

        _btnRefresh = new AppButton
        {
            Text = "تحديث",
            Variant = ButtonVariant.Secondary,
            Width = 90,
            Height = 36,
            Location = new Point(155, 24)
        };
        _btnRefresh.Click += async (s, e) => await LoadTeachersAsync();

        topPanel.Controls.Add(titleLabel);
        topPanel.Controls.Add(subtitleLabel);
        topPanel.Controls.Add(_btnAddTeacher);
        topPanel.Controls.Add(_btnRefresh);

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
        _btnSearch.Click += async (s, e) => await LoadTeachersAsync(_searchBox.Text);

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
            var url = string.IsNullOrWhiteSpace(query)
                ? "api/teachers?page=1&pageSize=50"
                : $"api/teachers?page=1&pageSize=50&searchTerm={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var paged = await response.Content.ReadFromJsonAsync<PagedResultModel<TeacherModel>>();
                var list = new List<object>();
                if (paged?.Items != null)
                {
                    int index = 1;
                    foreach (var t in paged.Items)
                    {
                        list.Add(new
                        {
                            SerialNumber = index++,
                            t.TeacherCode,
                            FullName = $"{t.FirstName} {t.LastName}",
                            Specialization = t.Subject,
                            t.PhoneNumber,
                            t.Email,
                            StatusText = t.Status == 0 ? "نشط" : "غير نشط"
                        });
                    }
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
        var specField = new FormField { LabelText = "المادة / التخصص *", Dock = DockStyle.Top };
        var phoneField = new FormField { LabelText = "رقم الهاتف *", Dock = DockStyle.Top };
        var emailField = new FormField { LabelText = "البريد الإلكتروني", Dock = DockStyle.Top };

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
                specialization = specField.Value.Trim(),
                phoneNumber = phoneField.Value.Trim(),
                email = emailField.Value.Trim()
            };

            try
            {
                var res = await _httpClient.PostAsJsonAsync("api/teachers", payload);
                if (res.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تسجيل المعلم بنجاح", "نجاح");
                    form.Close();
                    await LoadTeachersAsync();
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
        container.Controls.Add(emailField);
        container.Controls.Add(phoneField);
        container.Controls.Add(specField);
        container.Controls.Add(lastNameField);
        container.Controls.Add(firstNameField);
        container.Controls.Add(btnSave);

        form.Controls.Add(container);
        form.ShowDialog(this);
    }
}
