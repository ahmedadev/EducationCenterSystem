using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class AdminDashboardView
    {
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.BackgroundDark;
            this.RightToLeft = RightToLeft.Yes;

            // Top Header
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
                Text = "لوحة إدارة النظام والصلاحيات",
                Font = AppTheme.FontHero,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            titleContainer.Controls.Add(titleLabel);

            var actionContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0),
                AutoSize = true
            };

            _btnAddUser = new AppButton
            {
                Text = "+ إضافة مستخدم",
                Variant = ButtonVariant.Primary,
                Width = 140,
                Height = 40,
                Margin = new Padding(0, 0, 10, 0)
            };
            _btnAddUser.Click += async (s, e) => await OpenAddUserDialogAsync();

            _btnRefresh = new AppButton
            {
                Text = "تحديث",
                Variant = ButtonVariant.Secondary,
                Width = 90,
                Height = 40,
                Margin = new Padding(0)
            };
            _btnRefresh.Click += async (s, e) => await LoadDashboardDataAsync();

            actionContainer.Controls.Add(_btnAddUser);
            actionContainer.Controls.Add(_btnRefresh);

            topPanel.Controls.Add(titleContainer, 0, 0);
            topPanel.Controls.Add(actionContainer, 1, 0);

            // Stats Cards Bar
            var statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 95,
                BackColor = AppTheme.BackgroundDark,
                Padding = new Padding(16, 8, 16, 8),
                WrapContents = false
            };

            _cardUsers = new StatCard { Title = "إجمالي المستخدمين", Value = "0", ValueColor = AppTheme.TextPrimary };
            _cardActiveUsers = new StatCard { Title = "حسابات نشطة", Value = "0", ValueColor = AppTheme.StatusSuccess };
            _cardRoles = new StatCard { Title = "الأدوار المعرفة", Value = "0", ValueColor = AppTheme.AccentPrimary };
            _cardPermissions = new StatCard { Title = "إجمالي الصلاحيات", Value = "0", ValueColor = AppTheme.StatusWarning };

            statsPanel.Controls.Add(_cardUsers);
            statsPanel.Controls.Add(_cardActiveUsers);
            statsPanel.Controls.Add(_cardRoles);
            statsPanel.Controls.Add(_cardPermissions);

            // Users Grid
            _gridUsers = new DataGridView
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

            _gridUsers.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceCard;
            _gridUsers.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
            _gridUsers.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontBodyBold;
            _gridUsers.ColumnHeadersHeight = 38;

            _gridUsers.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
            _gridUsers.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            _gridUsers.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
            _gridUsers.DefaultCellStyle.SelectionForeColor = AppTheme.TextOnAccent;
            _gridUsers.DefaultCellStyle.Font = AppTheme.FontBody;
            _gridUsers.RowTemplate.Height = 36;

            _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 60 });
            _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "البريد الإلكتروني", DataPropertyName = "Email", FillWeight = 70 });
            _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "رقم الهاتف", DataPropertyName = "PhoneNumber", FillWeight = 40 });
            _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الدور", DataPropertyName = "RoleName", FillWeight = 40 });
            _gridUsers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحالة", DataPropertyName = "StatusText", FillWeight = 30 });

            this.Controls.Add(_gridUsers);
            this.Controls.Add(statsPanel);
            this.Controls.Add(topPanel);
        }
    }
}
