using EducationCenterSystem.Presentation.Models.DTOs;
using EducationCenterSystem.Presentation.Models.Constants;
using EducationCenterSystem.Presentation.Models.Enums;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EducationCenterSystem.Presentation.ViewModels;

public sealed partial class AttendanceViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<EducationalGroupDto> Groups { get; } = new();
    
    [ObservableProperty]
    private EducationalGroupDto? _selectedGroup;

    public ObservableCollection<GroupSessionDto> Sessions { get; } = new();

    [ObservableProperty]
    private GroupSessionDto? _selectedSession;

    public ObservableCollection<AttendanceRecordDto> AttendanceRecords { get; } = new();

    [ObservableProperty]
    private DateTime _newSessionDate = DateTime.Today;

    [ObservableProperty]
    private string _newSessionNotes = string.Empty;

    public AttendanceViewModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _ = LoadGroupsAsync();
    }

    [RelayCommand]
    private async Task LoadGroupsAsync()
    {
        IsBusy = true;
        try
        {
            Groups.Clear();
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
        finally
        {
            IsBusy = false;
        }
    }

    async partial void OnSelectedGroupChanged(EducationalGroupDto? value)
    {
        if (value is not null)
        {
            await LoadSessionsAsync(value.Id);
        }
        else
        {
            Sessions.Clear();
            AttendanceRecords.Clear();
        }
    }

    private async Task LoadSessionsAsync(Guid groupId)
    {
        IsBusy = true;
        try
        {
            Sessions.Clear();
            AttendanceRecords.Clear();
            var sessions = await _httpClient.GetFromJsonAsync<List<GroupSessionDto>>($"api/attendance/sessions/{groupId}");
            if (sessions != null)
            {
                foreach (var session in sessions)
                {
                    Sessions.Add(session);
                }
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    async partial void OnSelectedSessionChanged(GroupSessionDto? value)
    {
        if (value is not null)
        {
            await LoadAttendanceRecordsAsync(value.Id);
        }
        else
        {
            AttendanceRecords.Clear();
        }
    }

    private async Task LoadAttendanceRecordsAsync(Guid sessionId)
    {
        IsBusy = true;
        try
        {
            AttendanceRecords.Clear();
            var records = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>($"api/attendance/records/{sessionId}");
            if (records != null)
            {
                foreach (var record in records)
                {
                    AttendanceRecords.Add(record);
                }
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateSessionAsync()
    {
        if (SelectedGroup is null) return;

        IsBusy = true;
        try
        {
            var command = new { GroupId = SelectedGroup.Id, Date = NewSessionDate, Notes = NewSessionNotes };
            var response = await _httpClient.PostAsJsonAsync("api/attendance/sessions", command);

            if (response.IsSuccessStatusCode)
            {
                NewSessionNotes = string.Empty;
                await LoadSessionsAsync(SelectedGroup.Id);
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }
}

