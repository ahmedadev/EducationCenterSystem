using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationCenterSystem.Presentation.Services.Abstractions;

namespace EducationCenterSystem.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    private readonly RegisterStudentViewModel _registerStudentViewModel;
    private readonly StudentsListViewModel _studentsListViewModel;
    private readonly RegisterTeacherViewModel _registerTeacherViewModel;
    private readonly TeachersListViewModel _teachersListViewModel;
    private readonly AdminDashboardViewModel _adminDashboardViewModel;
    private readonly CoursesAndGroupsViewModel _coursesAndGroupsViewModel;
    private readonly AttendanceViewModel _attendanceViewModel;
    private readonly ITokenProvider _tokenProvider;

    public bool CanViewStudents => _tokenProvider.HasPermission(Permission.StudentsRead);
    public bool CanManageCourses => true;
    public bool CanRegisterStudent => _tokenProvider.HasPermission(Permission.StudentsCreate);
    public bool CanViewTeachers => _tokenProvider.HasPermission(Permission.TeachersRead);
    public bool CanRegisterTeacher => _tokenProvider.HasPermission(Permission.TeachersCreate);
    public bool CanManageRolesOrUsers => _tokenProvider.IsInRole("Admin") || 
                                         _tokenProvider.HasPermission(Permission.RolesManage) || 
                                         _tokenProvider.HasPermission(Permission.UsersManage) || 
                                         _tokenProvider.HasPermission(Permission.UsersRead);

    public string CurrentUserName => string.IsNullOrWhiteSpace(_tokenProvider.CurrentUserName) ? "مدير النظام" : _tokenProvider.CurrentUserName;
    public string CurrentUserRole => _tokenProvider.Roles.FirstOrDefault() ?? "Admin";

    public MainViewModel(
        RegisterStudentViewModel registerStudentViewModel,
        StudentsListViewModel studentsListViewModel,
        RegisterTeacherViewModel registerTeacherViewModel,
        TeachersListViewModel teachersListViewModel,
        AdminDashboardViewModel adminDashboardViewModel,
        CoursesAndGroupsViewModel coursesAndGroupsViewModel,
        AttendanceViewModel attendanceViewModel,
        ITokenProvider tokenProvider)
    {
        _registerStudentViewModel = registerStudentViewModel;
        _studentsListViewModel = studentsListViewModel;
        _registerTeacherViewModel = registerTeacherViewModel;
        _teachersListViewModel = teachersListViewModel;
        _adminDashboardViewModel = adminDashboardViewModel;
        _coursesAndGroupsViewModel = coursesAndGroupsViewModel;
        _attendanceViewModel = attendanceViewModel;
        _tokenProvider = tokenProvider;

        _studentsListViewModel.OnEditStudentRequested = (student) =>
        {
            _registerStudentViewModel.InitializeForEdit(student);
            CurrentViewModel = _registerStudentViewModel;
        };

        _registerStudentViewModel.OnSaveSuccess = () =>
        {
            CurrentViewModel = _studentsListViewModel;
            if (_studentsListViewModel.LoadStudentsCommand.CanExecute(null))
            {
                _studentsListViewModel.LoadStudentsCommand.Execute(null);
            }
        };

        _teachersListViewModel.OnEditTeacherRequested = (teacher) =>
        {
            _registerTeacherViewModel.InitializeForEdit(teacher);
            CurrentViewModel = _registerTeacherViewModel;
        };

        _registerTeacherViewModel.OnSaveSuccess = () =>
        {
            CurrentViewModel = _teachersListViewModel;
            if (_teachersListViewModel.LoadTeachersCommand.CanExecute(null))
            {
                _teachersListViewModel.LoadTeachersCommand.Execute(null);
            }
        };

        // Default view
        CurrentViewModel = _studentsListViewModel;
    }

    [RelayCommand]
    private void NavigateToRegister()
    {
        _registerStudentViewModel.InitializeForAdd();
        CurrentViewModel = _registerStudentViewModel;
    }

    [RelayCommand]
    private void NavigateToStudentsList()
    {
        CurrentViewModel = _studentsListViewModel;
    }

    [RelayCommand]
    private void NavigateToRegisterTeacher()
    {
        _registerTeacherViewModel.InitializeForAdd();
        CurrentViewModel = _registerTeacherViewModel;
    }

    [RelayCommand]
    private void NavigateToTeachersList()
    {
        CurrentViewModel = _teachersListViewModel;
    }

    [RelayCommand]
    private void NavigateToAdminDashboard()
    {
        CurrentViewModel = _adminDashboardViewModel;
    }

    [RelayCommand]
    private void NavigateToCoursesAndGroups()
    {
        CurrentViewModel = _coursesAndGroupsViewModel;
    }

    [RelayCommand]
    private void NavigateToAttendance()
    {
        CurrentViewModel = _attendanceViewModel;
    }
}

