using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class StudentsView : UserControl
{
    private readonly IStudentApiService _studentApiService;
    private readonly IDialogService _dialogService;
    private readonly HttpClient _httpClient;
    private readonly DataGridView _grid;
    private readonly TextBox _searchBox;
    private readonly AppButton _btnSearch;
    private readonly AppButton _btnAddStudent;
    private readonly AppButton _btnEnrollGroup;
    private readonly AppButton _btnRefresh;
    private readonly Label _statusLabel;

    private int _currentPage = 1;
    private int _totalPages = 1;
    private AppButton _btnNext = null!;
    private AppButton _btnPrev = null!;
    private AppButton _btnGoTo = null!;
    private Label _lblPageInfo = null!;
    private TextBox _txtGoToPage = null!;

    public StudentsView(IStudentApiService studentApiService, IDialogService dialogService, IHttpClientFactory httpClientFactory)
    {
        _studentApiService = studentApiService;
        _dialogService = dialogService;
        _httpClient = httpClientFactory.CreateClient();

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

        // Top Header & Action Bar
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
            Text = "إدارة الطلاب",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        var subtitleLabel = new Label
        {
            Text = "عرض بيانات الطلاب المسجلين والبحث وتعديل الحسابات",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 36)
        };
        titleContainer.Controls.Add(titleLabel);
        titleContainer.Controls.Add(subtitleLabel);

        var actionContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 10, 0, 0),
            AutoSize = true
        };

        _btnAddStudent = new AppButton
        {
            Text = "+ إضافة طالب",
            Variant = ButtonVariant.Primary,
            Width = 140,
            Height = 40,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnAddStudent.Click += (s, e) => OpenAddStudentDialog();

        _btnEnrollGroup = new AppButton
        {
            Text = "+ تسجيل في مجموعة",
            Variant = ButtonVariant.Secondary,
            Width = 160,
            Height = 40,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnEnrollGroup.Click += async (s, e) => await OpenEnrollStudentDialogAsync();

        _btnRefresh = new AppButton
        {
            Text = "تحديث",
            Variant = ButtonVariant.Secondary,
            Width = 100,
            Height = 40,
            Margin = new Padding(0)
        };
        _btnRefresh.Click += async (s, e) => { _currentPage = 1; await LoadStudentsAsync(); };

        actionContainer.Controls.Add(_btnAddStudent);
        actionContainer.Controls.Add(_btnEnrollGroup);
        actionContainer.Controls.Add(_btnRefresh);

        topPanel.Controls.Add(titleContainer, 0, 0);
        topPanel.Controls.Add(actionContainer, 1, 0);

        // Search Bar Panel
        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(16, 16, 16, 16),
            ColumnCount = 4,
            RowCount = 1,
            RightToLeft = RightToLeft.Yes
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300f));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110f));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _searchBox = new TextBox
        {
            Width = 290,
            Font = AppTheme.FontBody,
            BackColor = AppTheme.SurfaceCard,
            ForeColor = AppTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            Margin = new Padding(0, 4, 10, 0)
        };

        _btnSearch = new AppButton
        {
            Text = "بحث",
            Variant = ButtonVariant.Secondary,
            Width = 90,
            Height = 34,
            Margin = new Padding(0)
        };
        _btnSearch.Click += async (s, e) => { _currentPage = 1; await LoadStudentsAsync(_searchBox.Text); };

        _statusLabel = new Label
        {
            Text = "جاهز",
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.FontCaption,
            AutoSize = true,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            Margin = new Padding(0, 8, 0, 0)
        };

        searchPanel.Controls.Add(_searchBox, 0, 0);
        searchPanel.Controls.Add(_btnSearch, 1, 0);
        searchPanel.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 2, 0);
        searchPanel.Controls.Add(_statusLabel, 3, 0);

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
        
        // Enable Double Buffering to eliminate scrolling lag
        typeof(DataGridView).InvokeMember(
            "DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
            null,
            _grid,
            new object[] { true });

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

        var paginationPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = AppTheme.SurfaceCard,
            FlowDirection = FlowDirection.LeftToRight, // Starts from Right physically under RightToLeft.Yes
            WrapContents = false,
            Padding = new Padding(16, 10, 16, 10)
        };
        
        _btnPrev = new AppButton { Text = "< السابق", Width = 90, Height = 34, Variant = ButtonVariant.Secondary };
        _lblPageInfo = new Label { Text = "صفحة 1 من 1", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary, AutoSize = true, Margin = new Padding(10, 8, 10, 0) };
        _btnNext = new AppButton { Text = "التالي >", Width = 90, Height = 34, Variant = ButtonVariant.Secondary };
        
        var lblGoTo = new Label { Text = "انتقال إلى صفحة:", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Margin = new Padding(30, 8, 10, 0) };
        _txtGoToPage = new TextBox { Width = 50, Font = AppTheme.FontBody, BackColor = AppTheme.BackgroundDark, ForeColor = AppTheme.TextPrimary, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center, Margin = new Padding(0, 5, 0, 0) };
        _btnGoTo = new AppButton { Text = "انتقال", Width = 80, Height = 34, Variant = ButtonVariant.Primary, Margin = new Padding(10, 0, 0, 0) };

        _btnPrev.Click += async (s, e) => { if (_currentPage > 1) { _currentPage--; await LoadStudentsAsync(_searchBox.Text); } };
        _btnNext.Click += async (s, e) => { if (_currentPage < _totalPages) { _currentPage++; await LoadStudentsAsync(_searchBox.Text); } };
        _btnGoTo.Click += async (s, e) => 
        {
            if (int.TryParse(_txtGoToPage.Text, out int page) && page >= 1 && page <= _totalPages)
            {
                _currentPage = page;
                await LoadStudentsAsync(_searchBox.Text);
            }
        };

        paginationPanel.Controls.Add(_btnPrev);
        paginationPanel.Controls.Add(_lblPageInfo);
        paginationPanel.Controls.Add(_btnNext);
        paginationPanel.Controls.Add(lblGoTo);
        paginationPanel.Controls.Add(_txtGoToPage);
        paginationPanel.Controls.Add(_btnGoTo);

        Controls.Add(_grid);
        Controls.Add(paginationPanel);
        Controls.Add(searchPanel);
        Controls.Add(topPanel);

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
                var list = new List<object>();
                if (paged?.Items != null)
                {
                    int index = 1;
                    foreach (var s in paged.Items)
                    {
                        list.Add(new
                        {
                            s.Id,
                            SerialNumber = index++,
                            s.StudentCode,
                            FullName = $"{s.FirstName} {s.LastName}",
                            s.PhoneNumber,
                            s.ParentPhoneNumber,
                            s.GradeLevel,
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
        var studentId = (Guid)selectedRow.Cells["Id"].Value;
        var studentName = selectedRow.Cells["FullName"].Value?.ToString() ?? "الطالب";

        // Fetch groups
        List<EducationCenterSystem.Presentation.WinForms.Models.DTOs.EducationalGroupDto>? groups = null;
        try
        {
            var res = await _httpClient.GetAsync("api/educational-groups");
            if (res.IsSuccessStatusCode)
            {
                groups = await res.Content.ReadFromJsonAsync<List<EducationCenterSystem.Presentation.WinForms.Models.DTOs.EducationalGroupDto>>();
            }
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
                var payload = new { StudentId = studentId };
                var response = await _httpClient.PostAsJsonAsync($"api/educational-groups/{groupId}/students", payload);
                if (response.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم التسجيل بنجاح!", "نجاح");
                    form.Close();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    _dialogService.ShowError($"فشل التسجيل: {err}", "خطأ");
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
