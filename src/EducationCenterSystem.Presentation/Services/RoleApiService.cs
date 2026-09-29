using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class RoleApiService : IRoleApiService
{
    private readonly HttpClient _httpClient;

    public RoleApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<RoleModel>?> GetAllRolesAsync()
    {
        var response = await _httpClient.GetAsync("api/roles");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<RoleModel>>();
        }
        return null;
    }
}
