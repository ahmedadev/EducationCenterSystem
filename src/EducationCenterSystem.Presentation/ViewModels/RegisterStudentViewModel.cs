using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.ViewModels;

public partial class RegisterStudentViewModel : ObservableValidator
{
    private readonly HttpClient _httpClient;

    public Action? OnSaveSuccess { get; set; }

    [ObservableProperty]
    private Guid? _studentId;

    [ObservableProperty]
    private string _formTitle = "إضافة طالب جديد";

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
    private DateTime _dateOfBirth = DateTime.Now;

    [ObservableProperty]
    private string? _nationalId;

    [ObservableProperty]
    [Required(ErrorMessage = "رقم هاتف ولي الأمر مطلوب")]
    private string _parentPhoneNumber = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "المرحلة الدراسية مطلوبة")]
    private string _gradeLevel = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "كود الطالب مطلوب")]
    private string _studentCode = string.Empty;

    [ObservableProperty]
    private string? _schoolName;

    [ObservableProperty]
    private Gender _gender;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isBusy;

    private readonly IDialogService _dialogService;

    public RegisterStudentViewModel(IHttpClientFactory httpClientFactory, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _httpClient = httpClientFactory.CreateClient();
    }

    public void InitializeForEdit(Models.StudentModel student)
    {
        StudentId = student.Id;
        FormTitle = $"تعديل بيانات الطالب: {student.FirstName} {student.LastName}";

        FirstName = student.FirstName;
        LastName = student.LastName;
        Email = student.Email ?? string.Empty;
        PhoneNumber = student.PhoneNumber;
        DateOfBirth = student.DateOfBirth;
        NationalId = student.NationalId;
        ParentPhoneNumber = student.ParentPhoneNumber;
        GradeLevel = student.GradeLevel;
        StudentCode = student.StudentCode;
        SchoolName = student.SchoolName;
        // Assume Gender enum maps correctly
        Gender = student.Gender;
        Address = student.Address;
        Notes = student.Notes;
    }

    public void InitializeForAdd()
    {
        StudentId = null;
        FormTitle = "إضافة طالب جديد";

        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        DateOfBirth = DateTime.Now;
        NationalId = null;
        ParentPhoneNumber = string.Empty;
        GradeLevel = string.Empty;
        StudentCode = string.Empty;
        SchoolName = null;
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
            if (StudentId.HasValue)
            {
                // Update
                var request = new
                {
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    DateOfBirth = DateOfBirth.ToUniversalTime(),
                    NationalId,
                    ParentPhoneNumber,
                    GradeLevel,
                    StudentCode,
                    SchoolName,
                    Gender,
                    Address,
                    Notes
                };

                var response = await _httpClient.PutAsJsonAsync($"api/students/{StudentId.Value}", request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تحديث بيانات الطالب بنجاح!", "نجاح");
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
                // Create
                var command = new
                {
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    DateOfBirth = DateOfBirth.ToUniversalTime(),
                    NationalId,
                    ParentPhoneNumber,
                    GradeLevel,
                    StudentCode,
                    SchoolName,
                    Gender,
                    Address,
                    Notes
                };

                var response = await _httpClient.PostAsJsonAsync("api/students", command, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _dialogService.ShowInfo("تم تسجيل الطالب بنجاح!", "نجاح");
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

