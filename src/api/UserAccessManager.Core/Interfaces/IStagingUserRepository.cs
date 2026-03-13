using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IStagingUserRepository
{
    Task<PagedResult<StagingUserDto>> GetAllAsync(int page, int pageSize);
    Task<StagingUserDto?> GetByIdAsync(int id);
    Task<UserDto?> PromoteAsync(int stagingUserId, string createdBy);
}
