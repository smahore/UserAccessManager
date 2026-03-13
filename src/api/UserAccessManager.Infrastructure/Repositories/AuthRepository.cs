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
        const string sql = "EXEC GetUserRolesByUserName @UserName, @AllowedRoles";
        var results = await connection.QueryAsync<dynamic>(sql, new
        {
            request.UserName,
            request.AllowedRoles
        });

        return results.Select(r => new UserRoleDto
        {
            UserId = (int)r.UserId,
            AppName = (string)r.RoleName,
            AppId = 0
        });
    }
}
