using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public class TeachersView : UserControl
{
    private readonly ITeacherApiService _teacherApiService;
    private readonly IDialogService _dialogService;
    private readonly DataGridView _grid;
    private readonly TextBox _searchBox;
    private readonly AppButton _btnSearch;
    private readonly AppButton _btnAddTeacher;
    private readonly AppButton _btnRefresh;
    private readonly Label _statusLabel;

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

        Dock = DockStyle.Fill;
        BackColor = AppTheme.BackgroundDark;
        RightToLeft = RightToLeft.Yes;

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
            Text = "إدارة المعلمين",
            Font = AppTheme.FontHero,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        var subtitleLabel = new Label
        {
            Text = "سجل الكادر التعليمي، التخصصات، وأرقام التواصل",
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

        _btnAddTeacher = new AppButton
        {
            Text = "+ إضافة معلم",
            Variant = ButtonVariant.Primary,
            Width = 140,
            Height = 40,
            Margin = new Padding(0, 0, 10, 0)
        };
        _btnAddTeacher.Click += (s, e) => OpenAddTeacherDialog();

        _btnRefresh = new AppButton
        {
            Text = "تحديث",
            Variant = ButtonVariant.Secondary,
            Width = 100,
            Height = 40,
            Margin = new Padding(0)
        };
        _btnRefresh.Click += async (s, e) => { _currentPage = 1; await LoadTeachersAsync(); };

        actionContainer.Controls.Add(_btnAddTeacher);
        actionContainer.Controls.Add(_btnRefresh);

        topPanel.Controls.Add(titleContainer, 0, 0);
        topPanel.Controls.Add(actionContainer, 1, 0);

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
        _btnSearch.Click += async (s, e) => { _currentPage = 1; await LoadTeachersAsync(_searchBox.Text); };

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

        _btnPrev.Click += async (s, e) => { if (_currentPage > 1) { _currentPage--; await LoadTeachersAsync(_searchBox.Text); } };
        _btnNext.Click += async (s, e) => { if (_currentPage < _totalPages) { _currentPage++; await LoadTeachersAsync(_searchBox.Text); } };
        _btnGoTo.Click += async (s, e) => 
        {
            if (int.TryParse(_txtGoToPage.Text, out int page) && page >= 1 && page <= _totalPages)
            {
                _currentPage = page;
                await LoadTeachersAsync(_searchBox.Text);
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
