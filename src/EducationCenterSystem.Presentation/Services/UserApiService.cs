using System.Net.Http.Json;
using EducationCenterSystem.Presentation.WinForms.Models;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

namespace EducationCenterSystem.Presentation.WinForms.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<UserModel>?> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync("api/users");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<UserModel>>();
        }
        return null;
    }

    public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/assign-role", new { UserId = userId, RoleId = roleId });
        return response.IsSuccessStatusCode;
    }
}
