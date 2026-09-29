using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Theme;
using EducationCenterSystem.Presentation.WinForms.Components;
using EducationCenterSystem.Presentation.WinForms.Models.ViewModels;

namespace EducationCenterSystem.Presentation.WinForms.Views;

public partial class ParentsView : UserControl
{
    private readonly IParentApiService _parentApiService;
    private readonly IDialogService _dialogService;
    private DataGridView _grid = null!;
    private TextBox _searchBox = null!;
    private AppButton _btnSearch = null!;
    private AppButton _btnAddParent = null!;
    private AppButton _btnRefresh = null!;
    private Label _statusLabel = null!;

    private int _currentPage = 1;
    private int _totalPages = 1;
    private AppButton _btnNext = null!;
    private AppButton _btnPrev = null!;
    private Label _lblPageInfo = null!;

    public ParentsView(IParentApiService parentApiService, IDialogService dialogService)
    {
        _parentApiService = parentApiService;
        _dialogService = dialogService;

        InitializeComponent();
        _grid.ApplyModernTheme();
        _ = LoadParentsAsync();
    }

    private void ConfigureColumns()
    {
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", Visible = false });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "SerialNumber", FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم الكامل", Name = "FullName", DataPropertyName = "FullName", FillWeight = 70 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الهاتف", DataPropertyName = "PhoneNumber", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الوظيفة", DataPropertyName = "Job", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الرقم القومي", DataPropertyName = "NationalId", FillWeight = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "عدد الأبناء", DataPropertyName = "ChildrenCount", FillWeight = 30 });
    }

    private async Task LoadParentsAsync(string? query = null)
    {
        try
        {
            _statusLabel.Text = "جاري التحميل...";
            var paged = await _parentApiService.GetPagedParentsAsync(_currentPage, 20, query);
            
            if (paged != null)
            {
                var list = new List<ParentGridItemViewModel>();
                if (paged.Items != null)
                {
                    int index = 1;
                    foreach (var p in paged.Items)
                    {
                        list.Add(new ParentGridItemViewModel
                        {
                            Id = p.Id,
                            SerialNumber = index++,
                            FullName = $"{p.FirstName} {p.SecondName} {p.ThirdName} {p.LastName}".Replace("  ", " ").Trim(),
                            PhoneNumber = p.PhoneNumber,
                            Job = p.Job ?? "-",
                            NationalId = p.NationalId ?? "-",
                            ChildrenCount = p.ChildrenCount
                        });
                    }
                    _totalPages = paged.TotalPages;
                    _currentPage = paged.PageNumber;
                    _lblPageInfo.Text = $"صفحة {_currentPage} من {Math.Max(1, _totalPages)}";
                    _btnPrev.Enabled = _currentPage > 1;
                    _btnNext.Enabled = _currentPage < _totalPages;
                }
                _grid.DataSource = list;
                _statusLabel.Text = $"تم تحميل {list.Count} ولي أمر";
            }
            else
            {
                _statusLabel.Text = "فشل جلب البيانات من الخادم";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "خطأ في الاتصال";
            _dialogService.ShowError($"خطأ أثناء جلب البيانات: {ex.Message}", "خطأ");
        }
    }

    private void OpenAddParentDialog()
    {
        using var form = new Form
        {
            Text = "إضافة ولي أمر جديد",
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
        var secondNameField = new FormField { LabelText = "الاسم الثاني", Dock = DockStyle.Top };
        var thirdNameField = new FormField { LabelText = "الاسم الثالث", Dock = DockStyle.Top };
        var lastNameField = new FormField { LabelText = "الاسم الأخير *", Dock = DockStyle.Top };
        var phoneField = new FormField { LabelText = "رقم الهاتف *", Dock = DockStyle.Top };
        var nationalIdField = new FormField { LabelText = "الرقم القومي", Dock = DockStyle.Top };
        var jobField = new FormField { LabelText = "الوظيفة", Dock = DockStyle.Top };

        var btnSave = new AppButton
        {
            Text = "حفظ البيانات",
            Variant = ButtonVariant.Primary,
            Dock = DockStyle.Bottom,
            Height = 40
        };

        btnSave.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(firstNameField.Value) || string.IsNullOrWhiteSpace(lastNameField.Value) || string.IsNullOrWhiteSpace(phoneField.Value))
            {
                _dialogService.ShowError("الرجاء إدخال الاسم الأول، الأخير ورقم الهاتف", "تنبيه");
                return;
            }

            var payload = new ParentModel
            {
                FirstName = firstNameField.Value.Trim(),
                SecondName = secondNameField.Value.Trim(),
                ThirdName = thirdNameField.Value.Trim(),
                LastName = lastNameField.Value.Trim(),
                PhoneNumber = phoneField.Value.Trim(),
                NationalId = nationalIdField.Value.Trim(),
                Job = jobField.Value.Trim()
            };

            try
            {
                var success = await _parentApiService.CreateParentAsync(payload);
                if (success)
                {
                    _dialogService.ShowInfo("تم التسجيل بنجاح", "نجاح");
                    form.Close();
                    await LoadParentsAsync();
                }
                else
                {
                    _dialogService.ShowError("فشل التسجيل، تأكد من صحة البيانات (الرقم القومي أو الهاتف مسجل من قبل).", "خطأ");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"خطأ في الاتصال: {ex.Message}", "خطأ");
            }
        };

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        container.Controls.Add(jobField);
        container.Controls.Add(nationalIdField);
        container.Controls.Add(phoneField);
        container.Controls.Add(lastNameField);
        container.Controls.Add(thirdNameField);
        container.Controls.Add(secondNameField);
        container.Controls.Add(firstNameField);
        container.Controls.Add(btnSave);

        form.Controls.Add(container);
        form.ShowDialog(this);
    }
}
