using System.Drawing;
using System.Windows.Forms;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Views
{
    partial class CoursesAndGroupsView
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
                Text = "إدارة المواد والمجموعات التعليمية",
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

            _btnRefresh = new AppButton
            {
                Text = "تحديث الكل",
                Variant = ButtonVariant.Secondary,
                Width = 110,
                Height = 40,
                Margin = new Padding(0)
            };
            _btnRefresh.Click += async (s, e) => await LoadDataAsync();

            actionContainer.Controls.Add(_btnRefresh);

            topPanel.Controls.Add(titleContainer, 0, 0);
            topPanel.Controls.Add(actionContainer, 1, 0);

            // Main Layout (Split: Right is Forms, Left is Tables)
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 350,
                BackColor = AppTheme.BorderSubtle,
                FixedPanel = FixedPanel.Panel1
            };

            // Panel 1: Forms Panel
            var pnlForm = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.SurfaceCard,
                Padding = new Padding(16),
                AutoScroll = true
            };

            // --- ADD GROUP SECTION ---
            var pnlGroupForm = new Panel { Dock = DockStyle.Top, Height = 480, Padding = new Padding(0, 20, 0, 0) };
            var groupTitle = new Label { Text = "إضافة مجموعة تعليمية جديدة", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, Dock = DockStyle.Top, Height = 35 };

            var pnlCmbCourse = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
            var lblCourse = new Label { Text = "الكورس المرتبط *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
            _cmbCourses = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
            pnlCmbCourse.Controls.Add(_cmbCourses);
            pnlCmbCourse.Controls.Add(lblCourse);

            var pnlCmbTeacher = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.Transparent };
            var lblTeacher = new Label { Text = "معلم المجموعة *", Dock = DockStyle.Top, Height = 22, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontCaption, TextAlign = ContentAlignment.MiddleRight };
            _cmbTeachers = new ComboBox { Dock = DockStyle.Bottom, Height = 32, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = AppTheme.SurfaceCard, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBody };
            pnlCmbTeacher.Controls.Add(_cmbTeachers);
            pnlCmbTeacher.Controls.Add(lblTeacher);

            _txtGroupName = new FormField { LabelText = "اسم المجموعة (مثال: السبت والثلاثاء 5م) *", Dock = DockStyle.Top };
            _txtMaxCapacity = new FormField { LabelText = "الحد الأقصى للطلاب *", Dock = DockStyle.Top };
            _txtMonthlyFee = new FormField { LabelText = "الاشتراك الشهري *", Dock = DockStyle.Top };
            _txtSchedule = new FormField { LabelText = "مواعيد الحضور", Dock = DockStyle.Top };
            
            _btnSaveGroup = new AppButton { Text = "حفظ المجموعة", Variant = ButtonVariant.Primary, Dock = DockStyle.Top, Height = 40 };
            _btnSaveGroup.Click += async (s, e) => await SaveGroupAsync();

            pnlGroupForm.Controls.Add(_btnSaveGroup);
            pnlGroupForm.Controls.Add(_txtSchedule);
            pnlGroupForm.Controls.Add(_txtMonthlyFee);
            pnlGroupForm.Controls.Add(_txtMaxCapacity);
            pnlGroupForm.Controls.Add(_txtGroupName);
            pnlGroupForm.Controls.Add(pnlCmbTeacher);
            pnlGroupForm.Controls.Add(pnlCmbCourse);
            pnlGroupForm.Controls.Add(groupTitle);

            // --- ADD COURSE SECTION ---
            var pnlCourseForm = new Panel { Dock = DockStyle.Top, Height = 280, Padding = new Padding(0, 0, 0, 20) };
            var formTitle = new Label { Text = "إضافة مادة / كورس جديد", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, Dock = DockStyle.Top, Height = 35 };

            _txtCourseName = new FormField { LabelText = "اسم المادة / الكورس *", Dock = DockStyle.Top };
            _txtGradeLevel = new FormField { LabelText = "الصف الدراسي *", Dock = DockStyle.Top };
            _txtSubject = new FormField { LabelText = "المادة العلمية *", Dock = DockStyle.Top };

            _btnSaveCourse = new AppButton { Text = "حفظ الكورس", Variant = ButtonVariant.Primary, Dock = DockStyle.Top, Height = 40 };
            _btnSaveCourse.Click += async (s, e) => await SaveCourseAsync();

            pnlCourseForm.Controls.Add(_btnSaveCourse);
            pnlCourseForm.Controls.Add(_txtSubject);
            pnlCourseForm.Controls.Add(_txtGradeLevel);
            pnlCourseForm.Controls.Add(_txtCourseName);
            pnlCourseForm.Controls.Add(formTitle);

            pnlForm.Controls.Add(pnlGroupForm);
            pnlForm.Controls.Add(pnlCourseForm); 

            split.Panel1.Controls.Add(pnlForm);

            // Panel 2: Grids for Courses and Groups
            var pnlGrids = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.BackgroundDark,
                Padding = new Padding(12)
            };

            var lblCourses = new Label
            {
                Text = "المواد الدراسية الحالية",
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.AccentPrimary,
                Dock = DockStyle.Top,
                Height = 28
            };

            _gridCourses = CreateStyledGrid();
            _gridCourses.Dock = DockStyle.Top;
            _gridCourses.Height = 220;
            _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم الكورس", DataPropertyName = "Name", FillWeight = 60 });
            _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الصف الدراسي", DataPropertyName = "GradeLevel", FillWeight = 40 });
            _gridCourses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المادة", DataPropertyName = "Subject", FillWeight = 40 });

            var lblGroups = new Label
            {
                Text = "المجموعات التعليمية المرتبطة",
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.AccentPrimary,
                Dock = DockStyle.Top,
                Height = 35
            };

            _gridGroups = CreateStyledGrid();
            _gridGroups.Dock = DockStyle.Fill;
            _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم المجموعة", DataPropertyName = "Name", FillWeight = 50 });
            _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المادة", DataPropertyName = "CourseName", FillWeight = 40 });
            _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المعلم", DataPropertyName = "TeacherName", FillWeight = 40 });
            _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "السعة", DataPropertyName = "MaxCapacity", FillWeight = 25 });
            _gridGroups.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاشتراك الشهري", DataPropertyName = "MonthlyFee", FillWeight = 35 });

            pnlGrids.Controls.Add(_gridGroups);
            pnlGrids.Controls.Add(lblGroups);
            pnlGrids.Controls.Add(_gridCourses);
            pnlGrids.Controls.Add(lblCourses);
            split.Panel2.Controls.Add(pnlGrids);

            this.Controls.Add(split);
            this.Controls.Add(topPanel);
        }
    }
}
