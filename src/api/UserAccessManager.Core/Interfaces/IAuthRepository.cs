using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IAuthRepository
{
    Task<IEnumerable<UserRoleDto>> GetUserRolesByUserNameAsync(UserRolesLookupRequest request);
}
