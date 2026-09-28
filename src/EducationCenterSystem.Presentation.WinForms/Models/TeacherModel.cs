using EducationCenterSystem.Presentation.WinForms.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EducationCenterSystem.Presentation.WinForms.Models;

public partial class TeacherModel : ObservableObject
{
    [ObservableProperty]
    private int _serialNumber;

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private DateTime _dateOfBirth;

    [ObservableProperty]
    private DateTime _registeredOnUtc;

    [ObservableProperty]
    private string? _nationalId;

    [ObservableProperty]
    private string _teacherCode = string.Empty;

    [ObservableProperty]
    private string _subject = string.Empty;

    [ObservableProperty]
    private string? _qualification;

    [ObservableProperty]
    private Gender _gender;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private TeacherStatus _status;

    [ObservableProperty]
    private string? _notes;
}

