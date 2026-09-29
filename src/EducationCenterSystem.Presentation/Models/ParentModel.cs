using CommunityToolkit.Mvvm.ComponentModel;

namespace EducationCenterSystem.Presentation.WinForms.Models;

public partial class ParentModel : ObservableObject
{
    [ObservableProperty]
    private int _serialNumber;

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _secondName = string.Empty;

    [ObservableProperty]
    private string _thirdName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string? _email;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string? _nationalId;

    [ObservableProperty]
    private string? _job;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private DateTime _registeredOnUtc;

    [ObservableProperty]
    private int _childrenCount;
}
