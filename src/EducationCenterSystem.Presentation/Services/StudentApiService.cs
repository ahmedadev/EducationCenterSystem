using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class StudentApiService : IStudentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StudentApiService> _logger;
    private const string BaseUrl = "api/students";

    public StudentApiService(IHttpClientFactory httpClientFactory, ILogger<StudentApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<PagedResultModel<StudentModel>?> GetPagedStudentsAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(searchTerm)
                ? $"{BaseUrl}?page={page}&pageSize={pageSize}"
                : $"{BaseUrl}?page={page}&pageSize={pageSize}&searchTerm={Uri.EscapeDataString(searchTerm)}";
            
            return await _httpClient.GetFromJsonAsync<PagedResultModel<StudentModel>>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paged students from API.");
            return null;
        }
    }

    public async Task<StudentModel?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StudentModel>($"{BaseUrl}/{id}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching student {StudentId} from API.", id);
            return null;
        }
    }

    public async Task<bool> CreateStudentAsync(StudentModel student, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, student, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student via API.");
            return false;
        }
    }

    public async Task<bool> UpdateStudentAsync(Guid id, StudentModel student, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", student, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId} via API.", id);
            return false;
        }
    }

    public async Task<bool> DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId} via API.", id);
            return false;
        }
    }
}
