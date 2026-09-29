using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class AttendanceView
    {
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.BackgroundDark;
            this.RightToLeft = RightToLeft.Yes;

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

            this.Controls.Add(_grid);
            this.Controls.Add(filterPanel);
            this.Controls.Add(topPanel);
        }
    }
}
