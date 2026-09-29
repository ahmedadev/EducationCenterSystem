using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class EducationalGroupApiService : IEducationalGroupApiService
{
    private readonly HttpClient _httpClient;

    public EducationalGroupApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<EducationalGroupDto>?> GetAllGroupsAsync()
    {
        var response = await _httpClient.GetAsync("api/educational-groups");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<EducationalGroupDto>>();
        }
        return null;
    }

    public async Task<List<EducationalGroupStudentDto>?> GetGroupStudentsAsync(Guid groupId)
    {
        var response = await _httpClient.GetAsync($"api/educational-groups/{groupId}/students");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<EducationalGroupStudentDto>>();
        }
        return null;
    }

    public async Task<bool> EnrollStudentAsync(Guid groupId, Guid studentId)
    {
        var payload = new { StudentId = studentId };
        var response = await _httpClient.PostAsJsonAsync($"api/educational-groups/{groupId}/students", payload);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateGroupAsync(object payload)
    {
        var response = await _httpClient.PostAsJsonAsync("api/educational-groups", payload);
        return response.IsSuccessStatusCode;
    }
}
