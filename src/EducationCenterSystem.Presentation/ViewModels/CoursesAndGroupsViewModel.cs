using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EducationCenterSystem.Presentation.ViewModels;

public sealed partial class CoursesAndGroupsViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private int _selectedTabIndex;
    
    // Courses Data
    public ObservableCollection<CourseDto> Courses { get; } = new();
    private CourseDto? _selectedCourse;
    public CourseDto? SelectedCourse
    {
        get => _selectedCourse;
        set => SetProperty(ref _selectedCourse, value);
    }
    
    // Groups Data
    public ObservableCollection<EducationalGroupDto> Groups { get; } = new();
    
    // Form Inputs - Course
    private string _newCourseName = string.Empty;
    public string NewCourseName { get => _newCourseName; set => SetProperty(ref _newCourseName, value); }
    
    private string _newCourseGradeLevel = string.Empty;
    public string NewCourseGradeLevel { get => _newCourseGradeLevel; set => SetProperty(ref _newCourseGradeLevel, value); }
    
    private string _newCourseSubject = string.Empty;
    public string NewCourseSubject { get => _newCourseSubject; set => SetProperty(ref _newCourseSubject, value); }
    
    // Form Inputs - Group
    private string _newGroupName = string.Empty;
    public string NewGroupName { get => _newGroupName; set => SetProperty(ref _newGroupName, value); }
    
    // UI State
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public CoursesAndGroupsViewModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void SwitchTab(string tab)
    {
        if (int.TryParse(tab, out int index))
            SelectedTabIndex = index;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadCoursesAsync();
            await LoadGroupsAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoursesAsync()
    {
        Courses.Clear();
        try 
        {
            var courses = await _httpClient.GetFromJsonAsync<List<CourseDto>>("api/courses");
            if (courses != null)
            {
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
        }
        catch { }
    }

    private async Task LoadGroupsAsync()
    {
        Groups.Clear();
        try 
        {
            var groups = await _httpClient.GetFromJsonAsync<List<EducationalGroupDto>>("api/educationalgroups");
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    Groups.Add(group);
                }
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task CreateCourseAsync()
    {
        IsBusy = true;
        try
        {
            var command = new { Name = NewCourseName, GradeLevel = NewCourseGradeLevel, Subject = NewCourseSubject, Description = "Created from UI" };
            var response = await _httpClient.PostAsJsonAsync("api/courses", command);
            
            if (response.IsSuccessStatusCode)
            {
                NewCourseName = string.Empty;
                NewCourseGradeLevel = string.Empty;
                NewCourseSubject = string.Empty;
                await LoadCoursesAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateGroupAsync()
    {
        await Task.CompletedTask;
        // Simple stub for demo
        IsBusy = true;
        try
        {
            // Requires TeacherId and CourseId which would normally be selected via ComboBoxes
            // This is just a structural implementation per requirements.
        }
        finally
        {
            IsBusy = false;
        }
    }
}

