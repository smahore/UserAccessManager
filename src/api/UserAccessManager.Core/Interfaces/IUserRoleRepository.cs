using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(int userId);
    Task<int> AssignRoleAsync(int userId, int appId);
    Task<bool> RemoveRoleAsync(int userId, int appId);
}
