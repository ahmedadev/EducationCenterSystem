using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationCenterSystem.Presentation.Models;
using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.ViewModels;

public partial class RegisterTeacherViewModel : ObservableValidator
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService _dialogService;

    public Action? OnSaveSuccess { get; set; }

    [ObservableProperty]
    private Guid? _teacherId;

    [ObservableProperty]
    private string _formTitle = "إضافة معلم جديد";

    [ObservableProperty]
    [Required(ErrorMessage = "كود المعلم مطلوب")]
    private string _teacherCode = string.Empty;

    [ObservableProperty]
    private string? _nationalId;

    [ObservableProperty]
    [Required(ErrorMessage = "الاسم الأول مطلوب")]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "الاسم الأخير مطلوب")]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
    private DateTime _dateOfBirth = DateTime.Now.AddYears(-25);

    [ObservableProperty]
    [Required(ErrorMessage = "المادة التخصصية مطلوبة")]
    private string _subject = string.Empty;

    [ObservableProperty]
    private string? _qualification;

    [ObservableProperty]
    private Gender _gender = Gender.Male;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isBusy;

    public RegisterTeacherViewModel(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _dialogService = dialogService;
    }

    public void InitializeForEdit(TeacherModel teacher)
    {
        TeacherId = teacher.Id;
        FormTitle = $"تعديل بيانات المعلم: {teacher.FirstName} {teacher.LastName}";

        TeacherCode = teacher.TeacherCode;
        NationalId = teacher.NationalId;
        FirstName = teacher.FirstName;
        LastName = teacher.LastName;
        Email = teacher.Email;
        PhoneNumber = teacher.PhoneNumber;
        DateOfBirth = teacher.DateOfBirth;
        Subject = teacher.Subject;
        Qualification = teacher.Qualification;
        Gender = teacher.Gender;
        Address = teacher.Address;
        Notes = teacher.Notes;
        ClearErrors();
    }

    public void InitializeForAdd()
    {
        TeacherId = null;
        FormTitle = "إضافة معلم جديد";

        TeacherCode = string.Empty;
        NationalId = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        DateOfBirth = DateTime.Now.AddYears(-25);
        Subject = string.Empty;
        Qualification = null;
        Gender = Gender.Male;
        Address = null;
        Notes = null;
        ClearErrors();
    }

    [RelayCommand]
    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        ValidateAllProperties();
        if (HasErrors) return;

        IsBusy = true;
        try
        {
            if (TeacherId.HasValue)
            {
                var request = new
                {
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    DateOfBirth = DateTime.SpecifyKind(DateOfBirth, DateTimeKind.Utc),
                    NationalId,
                    TeacherCode,
                    Subject,
                    Qualification,
                    Gender,
                    Address,
                    Notes
                };

                var response = await _httpClient.PutAsJsonAsync($"api/teachers/{TeacherId.Value}", request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تحديث بيانات المعلم بنجاح!", "نجاح");
                    OnSaveSuccess?.Invoke();
                }
                else
                {
                    var errorString = await response.Content.ReadAsStringAsync(cancellationToken);
                    _dialogService.ShowError($"فشل التحديث:\n{errorString}", "خطأ");
                }
            }
            else
            {
                var command = new 
                {
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    DateOfBirth = DateTime.SpecifyKind(DateOfBirth, DateTimeKind.Utc),
                    NationalId,
                    TeacherCode,
                    Subject,
                    Qualification,
                    Gender,
                    Address,
                    Notes
                };

                var response = await _httpClient.PostAsJsonAsync("api/teachers", command, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تسجيل المعلم بنجاح!", "نجاح");
                    InitializeForAdd();
                    OnSaveSuccess?.Invoke();
                }
                else
                {
                    var errorString = await response.Content.ReadAsStringAsync(cancellationToken);
                    _dialogService.ShowError($"فشل التسجيل:\n{errorString}", "خطأ");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"حدث خطأ أثناء الاتصال بالخادم:\n{ex.Message}", "خطأ");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

