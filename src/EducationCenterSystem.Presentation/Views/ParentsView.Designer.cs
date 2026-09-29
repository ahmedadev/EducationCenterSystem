namespace EducationCenterSystem.Presentation.WinForms.Views;

partial class ParentsView
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        
        // Setup Grid
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.SurfaceCard,
            ForeColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.TextPrimary,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.None,
            GridColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.BorderSubtle,
            ColumnHeadersHeight = 40,
            RowTemplate = { Height = 45 }
        };

        // Header Panel
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(20, 10, 20, 10) };
        var lblTitle = new Label 
        { 
            Text = "إدارة أولياء الأمور", 
            Dock = DockStyle.Right, 
            Font = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.FontTitle,
            ForeColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.TextPrimary,
            AutoSize = true
        };

        // Actions Panel
        var pnlActions = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(20, 10, 20, 10) };
        
        _searchBox = new TextBox 
        { 
            Width = 300,
            Height = 35,
            Font = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.FontBody,
            BackColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.SurfaceCard,
            ForeColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.TextPrimary,
            PlaceholderText = "بحث بالاسم، الهاتف، أو الرقم القومي..."
        };
        _searchBox.Location = new Point(20, 12);

        _btnSearch = new EducationCenterSystem.Presentation.WinForms.Components.AppButton
        {
            Text = "بحث",
            Variant = EducationCenterSystem.Presentation.WinForms.Components.ButtonVariant.Secondary,
            Width = 100,
            Height = 35,
            Location = new Point(330, 10)
        };
        _btnSearch.Click += async (s, e) => { _currentPage = 1; await LoadParentsAsync(_searchBox.Text); };

        _btnAddParent = new EducationCenterSystem.Presentation.WinForms.Components.AppButton
        {
            Text = "إضافة ولي أمر",
            Variant = EducationCenterSystem.Presentation.WinForms.Components.ButtonVariant.Primary,
            Width = 150,
            Height = 35,
            Dock = DockStyle.Right
        };
        _btnAddParent.Click += (s, e) => OpenAddParentDialog();

        _btnRefresh = new EducationCenterSystem.Presentation.WinForms.Components.AppButton
        {
            Text = "تحديث",
            Variant = EducationCenterSystem.Presentation.WinForms.Components.ButtonVariant.Secondary,
            Width = 100,
            Height = 35,
            Dock = DockStyle.Right
        };
        _btnRefresh.Click += async (s, e) => await LoadParentsAsync(_searchBox.Text);

        pnlHeader.Controls.Add(lblTitle);
        
        pnlActions.Controls.Add(_searchBox);
        pnlActions.Controls.Add(_btnSearch);
        pnlActions.Controls.Add(_btnRefresh);
        pnlActions.Controls.Add(_btnAddParent);

        // Footer Panel (Pagination & Status)
        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(20, 10, 20, 10) };
        
        _statusLabel = new Label
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            ForeColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.TextSecondary,
            Font = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.FontCaption
        };

        var pnlPagination = new FlowLayoutPanel 
        { 
            Dock = DockStyle.Left, 
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        _btnNext = new EducationCenterSystem.Presentation.WinForms.Components.AppButton { Text = "< السابق", Width = 80, Height = 35, Variant = EducationCenterSystem.Presentation.WinForms.Components.ButtonVariant.Secondary };
        _btnPrev = new EducationCenterSystem.Presentation.WinForms.Components.AppButton { Text = "التالي >", Width = 80, Height = 35, Variant = EducationCenterSystem.Presentation.WinForms.Components.ButtonVariant.Secondary };
        _btnNext.Click += async (s, e) => { if (_currentPage < _totalPages) { _currentPage++; await LoadParentsAsync(_searchBox.Text); } };
        _btnPrev.Click += async (s, e) => { if (_currentPage > 1) { _currentPage--; await LoadParentsAsync(_searchBox.Text); } };

        _lblPageInfo = new Label { Text = "صفحة 1 من 1", AutoSize = true, Anchor = AnchorStyles.None, Margin = new Padding(10), ForeColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.TextPrimary };

        pnlPagination.Controls.Add(_btnPrev);
        pnlPagination.Controls.Add(_lblPageInfo);
        pnlPagination.Controls.Add(_btnNext);

        pnlFooter.Controls.Add(_statusLabel);
        pnlFooter.Controls.Add(pnlPagination);

        // Main Container
        var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 0, 20, 0) };
        pnlMain.Controls.Add(_grid);

        // Layout Assembly
        this.Controls.Add(pnlMain);
        this.Controls.Add(pnlActions);
        this.Controls.Add(pnlHeader);
        this.Controls.Add(pnlFooter);
        
        this.BackColor = EducationCenterSystem.Presentation.WinForms.Theme.AppTheme.BackgroundDark;
        this.RightToLeft = RightToLeft.Yes;
        this.Size = new Size(1000, 700);

        ConfigureColumns();
        
        this.ResumeLayout(false);
    }
}
