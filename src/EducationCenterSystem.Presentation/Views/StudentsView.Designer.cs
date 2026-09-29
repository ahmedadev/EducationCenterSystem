using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class StudentsView
    {
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.BackgroundDark;
            this.RightToLeft = RightToLeft.Yes;

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

            _btnLinkParent = new AppButton
            {
                Text = "ربط بولي أمر",
                Variant = ButtonVariant.Secondary,
                Width = 140,
                Height = 40,
                Margin = new Padding(0, 0, 10, 0)
            };
            _btnLinkParent.Click += async (s, e) => await OpenLinkParentDialogAsync();

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
            actionContainer.Controls.Add(_btnLinkParent);
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
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };
            
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
                FlowDirection = FlowDirection.LeftToRight,
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

            this.Controls.Add(_grid);
            this.Controls.Add(paginationPanel);
            this.Controls.Add(searchPanel);
            this.Controls.Add(topPanel);
        }
    }
}
