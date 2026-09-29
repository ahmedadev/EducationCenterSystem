using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class CourseApiService : ICourseApiService
{
    private readonly HttpClient _httpClient;

    public CourseApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<CourseDto>?> GetAllCoursesAsync()
    {
        var response = await _httpClient.GetAsync("api/courses");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<CourseDto>>();
        }
        return null;
    }

    public async Task<bool> CreateCourseAsync(object payload)
    {
        var response = await _httpClient.PostAsJsonAsync("api/courses", payload);
        return response.IsSuccessStatusCode;
    }
}
