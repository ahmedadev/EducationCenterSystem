using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Models.DTOs;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class ParentApiService : IParentApiService
{
    private readonly HttpClient _httpClient;

    public ParentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ParentDto>?> GetAllParentsAsync()
    {
        var response = await _httpClient.GetAsync("api/parents");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ParentDto>>();
        }
        return null;
    }

    public async Task<PagedResultModel<ParentDto>?> GetPagedParentsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
        }

        var response = await _httpClient.GetAsync($"api/parents/paged{query}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PagedResultModel<ParentDto>>();
        }
        return null;
    }

    public async Task<bool> CreateParentAsync(ParentModel parent)
    {
        var response = await _httpClient.PostAsJsonAsync("api/parents", parent);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateParentAsync(ParentModel parent)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/parents/{parent.Id}", parent);
        return response.IsSuccessStatusCode;
    }
}
