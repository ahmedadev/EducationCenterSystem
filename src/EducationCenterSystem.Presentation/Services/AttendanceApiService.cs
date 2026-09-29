using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class AttendanceApiService : IAttendanceApiService
{
    private readonly HttpClient _httpClient;

    public AttendanceApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<bool> SubmitBatchAttendanceAsync(object payload)
    {
        var response = await _httpClient.PostAsJsonAsync("api/attendance/batch", payload);
        return response.IsSuccessStatusCode;
    }
}
