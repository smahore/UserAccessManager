using Dapper;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly DapperContext _context;

    public AuthRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserRoleDto>> GetUserRolesByUserNameAsync(UserRolesLookupRequest request)
    {
        using var connection = _context.CreateConnection();
        var results = await connection.QueryAsync<SpUserRole>(
            "EXEC GetUserRolesByUserName @UserName, @AllowedRoles",
            new { request.UserName, request.AllowedRoles });

        return results.Select(r => new UserRoleDto
        {
            UserId = r.UserId,
            AppName = r.RoleName,
            AppId = 0
        });
    }

    private sealed record SpUserRole(int UserId, string RoleName);
}
