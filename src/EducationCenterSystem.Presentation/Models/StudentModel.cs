using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EducationCenterSystem.Presentation.Models;

public partial class StudentModel : ObservableObject
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
    private string? _nationalId;

    [ObservableProperty]
    private string _parentPhoneNumber = string.Empty;

    [ObservableProperty]
    private string _gradeLevel = string.Empty;

    [ObservableProperty]
    private string _studentCode = string.Empty;

    [ObservableProperty]
    private string? _schoolName;

    [ObservableProperty]
    private Gender _gender;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private StudentStatus _status;

    [ObservableProperty]
    private string? _notes;
}

