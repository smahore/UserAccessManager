using System.Net.Http.Json;
using UserAccessManager.Web.Models;

namespace UserAccessManager.Web.Services;

/// <summary>
/// Typed HTTP client for communicating with the UserAccessManager API.
/// </summary>
public class ApiClientService
{
    private readonly HttpClient _http;

    public ApiClientService(HttpClient http) => _http = http;

    // ── Users ──────────────────────────────────────────────────────────

    public async Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var url = $"api/users?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";
        return await GetDataAsync<PagedResult<UserDto>>(url);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
        => await GetDataOrDefaultAsync<UserDto>($"api/users/{id}");

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request)
        => await PostAsync<UserDto>("api/users", request);

    public async Task<ApiResponse<object>> UpdateUserAsync(int id, UpdateUserRequest request)
        => await PutAsync<object>($"api/users/{id}", request);

    public async Task<ApiResponse<object>> UpdateUserStatusAsync(int id, bool isActive)
        => await PatchAsync<object>($"api/users/{id}/status", isActive);

    // ── Access Requests ────────────────────────────────────────────────

    public async Task<PagedResult<AccessRequestDto>> GetAccessRequestsAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        var url = $"api/access-requests?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(status))
            url += $"&status={Uri.EscapeDataString(status)}";
        return await GetDataAsync<PagedResult<AccessRequestDto>>(url);
    }

    public async Task<AccessRequestDto?> GetAccessRequestByIdAsync(int id)
        => await GetDataOrDefaultAsync<AccessRequestDto>($"api/access-requests/{id}");

    public async Task<ApiResponse<AccessRequestResultDto>> CreateAccessRequestAsync(CreateAccessRequest request)
        => await PostAsync<AccessRequestResultDto>("api/access-requests", request);

    public async Task<ApiResponse<object>> UpdateAccessRequestStatusAsync(int id, string status)
        => await PatchAsync<object>($"api/access-requests/{id}/status", new UpdateStatusRequest { Status = status });

    // ── Applications ───────────────────────────────────────────────────

    public async Task<List<ApplicationDto>> GetApplicationsAsync()
        => await GetDataAsync<List<ApplicationDto>>("api/applications");

    public async Task<ApplicationDto?> GetApplicationByIdAsync(int id)
        => await GetDataOrDefaultAsync<ApplicationDto>($"api/applications/{id}");

    public async Task<ApiResponse<ApplicationDto>> CreateApplicationAsync(CreateApplicationRequest request)
        => await PostAsync<ApplicationDto>("api/applications", request);

    public async Task<ApiResponse<object>> UpdateApplicationAsync(int id, UpdateApplicationRequest request)
        => await PutAsync<object>($"api/applications/{id}", request);

    // ── Staging Users ──────────────────────────────────────────────────

    public async Task<PagedResult<StagingUserDto>> GetStagingUsersAsync(int page = 1, int pageSize = 20)
        => await GetDataAsync<PagedResult<StagingUserDto>>($"api/staging-users?page={page}&pageSize={pageSize}");

    public async Task<ApiResponse<UserDto>> PromoteStagingUserAsync(int id, string? createdBy = null)
        => await PostAsync<UserDto>($"api/staging-users/promote/{id}", new PromoteUserRequest { CreatedBy = createdBy });

    // ── User Roles ─────────────────────────────────────────────────────

    public async Task<List<UserRoleDto>> GetUserRolesAsync(int userId)
        => await GetDataAsync<List<UserRoleDto>>($"api/users/{userId}/roles");

    public async Task<ApiResponse<UserRoleDto>> AssignRoleAsync(int userId, int appId)
        => await PostAsync<UserRoleDto>($"api/users/{userId}/roles", new AssignRoleRequest { AppId = appId });

    public async Task<ApiResponse<object>> RemoveRoleAsync(int userId, int appId)
        => await DeleteAsync<object>($"api/users/{userId}/roles/{appId}");

    // ── Private helpers ────────────────────────────────────────────────

    private async Task<T> GetDataAsync<T>(string url) where T : new()
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<T>>(url);
        return response is { Data: not null } ? response.Data : new T();
    }

    private async Task<T?> GetDataOrDefaultAsync<T>(string url)
    {
        var httpResponse = await _http.GetAsync(url);
        if (!httpResponse.IsSuccessStatusCode)
            return default;
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return response is { Success: true } ? response.Data : default;
    }

    private async Task<ApiResponse<T>> PostAsync<T>(string url, object payload)
    {
        var httpResponse = await _http.PostAsJsonAsync(url, payload);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>()
               ?? new ApiResponse<T> { Success = false, Message = "No response from API." };
    }

    private async Task<ApiResponse<T>> PutAsync<T>(string url, object payload)
    {
        var httpResponse = await _http.PutAsJsonAsync(url, payload);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>()
               ?? new ApiResponse<T> { Success = false, Message = "No response from API." };
    }

    private async Task<ApiResponse<T>> PatchAsync<T>(string url, object payload)
    {
        var httpResponse = await _http.PatchAsJsonAsync(url, payload);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>()
               ?? new ApiResponse<T> { Success = false, Message = "No response from API." };
    }

    private async Task<ApiResponse<T>> DeleteAsync<T>(string url)
    {
        var httpResponse = await _http.DeleteAsync(url);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>()
               ?? new ApiResponse<T> { Success = false, Message = "No response from API." };
    }
}
