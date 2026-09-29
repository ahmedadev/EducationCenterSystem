using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class TeacherApiService : ITeacherApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeacherApiService> _logger;
    private const string BaseUrl = "api/teachers";

    public TeacherApiService(IHttpClientFactory httpClientFactory, ILogger<TeacherApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<List<TeacherModel>> GetAllTeachersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TeacherModel>>(BaseUrl, cancellationToken);
            return response ?? new List<TeacherModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teachers from API.");
            return new List<TeacherModel>();
        }
    }

    public async Task<PagedResultModel<TeacherModel>?> GetPagedTeachersAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(searchTerm)
                ? $"{BaseUrl}?page={page}&pageSize={pageSize}"
                : $"{BaseUrl}?page={page}&pageSize={pageSize}&searchTerm={Uri.EscapeDataString(searchTerm)}";
            
            return await _httpClient.GetFromJsonAsync<PagedResultModel<TeacherModel>>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paged teachers from API.");
            return null;
        }
    }

    public async Task<TeacherModel?> GetTeacherByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TeacherModel>($"{BaseUrl}/{id}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teacher {TeacherId} from API.", id);
            return null;
        }
    }

    public async Task<bool> CreateTeacherAsync(TeacherModel teacher, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, teacher, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating teacher via API.");
            return false;
        }
    }

    public async Task<bool> UpdateTeacherAsync(Guid id, TeacherModel teacher, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", teacher, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teacher {TeacherId} via API.", id);
            return false;
        }
    }

    public async Task<bool> DeleteTeacherAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting teacher {TeacherId} via API.", id);
            return false;
        }
    }
}
