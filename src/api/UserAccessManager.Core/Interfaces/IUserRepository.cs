using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IUserRepository
{
    Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, string? search = null);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(CreateUserRequest request);
    Task<bool> UpdateAsync(int id, UpdateUserRequest request);
    Task<bool> UpdateStatusAsync(int id, bool isActive);
}
