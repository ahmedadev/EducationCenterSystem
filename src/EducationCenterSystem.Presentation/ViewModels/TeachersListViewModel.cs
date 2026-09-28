using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationCenterSystem.Presentation.Models;
using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.ViewModels;

public partial class TeachersListViewModel : ObservableValidator
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<TeacherModel> _teachers = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreviousPage))]
    [NotifyPropertyChangedFor(nameof(HasNextPage))]
    [NotifyPropertyChangedFor(nameof(PageSummaryText))]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _pageSize = 15;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNextPage))]
    [NotifyPropertyChangedFor(nameof(PageSummaryText))]
    private int _totalPages = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageSummaryText))]
    private int _totalCount = 0;

    [ObservableProperty]
    private string _targetPageInput = "1";

    public ObservableCollection<int> PageSizeOptions { get; } = new() { 10, 15, 20, 25, 50 };

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public string PageSummaryText => $"صفحة {CurrentPage:N0} من {TotalPages:N0}  (إجمالي {TotalCount:N0} معلم)";

    public Action<TeacherModel>? OnEditTeacherRequested { get; set; }

    public TeachersListViewModel(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
        _ = LoadPageAsync(1);
    }

    partial void OnPageSizeChanged(int value)
    {
        _ = LoadPageAsync(1);
    }

    [RelayCommand]
    private async Task LoadTeachersAsync(CancellationToken cancellationToken = default)
    {
        await LoadPageAsync(CurrentPage, cancellationToken);
    }

    public async Task LoadPageAsync(int page, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        IsBusy = true;
        try
        {
            var url = $"api/teachers?page={page}&pageSize={PageSize}";
            var response = await _httpClient.GetFromJsonAsync<PagedResultModel<TeacherModel>>(url, cancellationToken);
            if (response is not null)
            {
                Teachers.Clear();
                int index = (response.PageNumber - 1) * response.PageSize + 1;
                foreach (var teacher in response.Items)
                {
                    teacher.SerialNumber = index++;
                    Teachers.Add(teacher);
                }

                CurrentPage = response.PageNumber;
                TotalPages = response.TotalPages > 0 ? response.TotalPages : 1;
                TotalCount = response.TotalCount;
                TargetPageInput = response.PageNumber.ToString();
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"خطأ في جلب بيانات الصفحة {page}:\n{ex.Message}", "خطأ في الشبكة");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (HasNextPage)
        {
            await LoadPageAsync(CurrentPage + 1);
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (HasPreviousPage)
        {
            await LoadPageAsync(CurrentPage - 1);
        }
    }

    [RelayCommand]
    private async Task FirstPageAsync()
    {
        if (CurrentPage != 1)
        {
            await LoadPageAsync(1);
        }
    }

    [RelayCommand]
    private async Task LastPageAsync()
    {
        if (CurrentPage != TotalPages)
        {
            await LoadPageAsync(TotalPages);
        }
    }

    [RelayCommand]
    private async Task JumpToPageAsync()
    {
        if (int.TryParse(TargetPageInput, out int targetPage))
        {
            if (targetPage >= 1 && targetPage <= TotalPages)
            {
                await LoadPageAsync(targetPage);
            }
            else
            {
                _dialogService.ShowInfo($"رقم الصفحة يجب أن يكون بين 1 و {TotalPages:N0}", "تنبيه");
                TargetPageInput = CurrentPage.ToString();
            }
        }
        else
        {
            _dialogService.ShowInfo("يرجى إدخال رقم صفحة صحيح.", "تنبيه");
            TargetPageInput = CurrentPage.ToString();
        }
    }

    [RelayCommand]
    private void EditTeacher(TeacherModel? teacher)
    {
        if (teacher is null) return;
        OnEditTeacherRequested?.Invoke(teacher);
    }

    [RelayCommand]
    private async Task DeleteTeacherAsync(TeacherModel? teacher)
    {
        if (teacher is null) return;

        bool confirmed = _dialogService.Confirm(
            $"هل أنت متأكد من حذف المعلم {teacher.FirstName} {teacher.LastName}؟",
            "تأكيد الحذف");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var response = await _httpClient.DeleteAsync($"api/teachers/{teacher.Id}");
            if (response.IsSuccessStatusCode)
            {
                _dialogService.ShowInfo("تم حذف المعلم بنجاح.", "تم الحذف");
                await LoadPageAsync(CurrentPage);
            }
            else
            {
                _dialogService.ShowError("فشل حذف المعلم من السيرفر.", "خطأ");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"حدث خطأ أثناء محاولة الحذف:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
