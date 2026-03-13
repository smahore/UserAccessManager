using Dapper;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly DapperContext _context;

    public UserRoleRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(int userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            SELECT ur.UserRoleId, ur.UserId, ur.AppId, a.AppName, ur.CreatedAt
            FROM UserRoles ur
            INNER JOIN ApplicationName a ON ur.AppId = a.AppId
            WHERE ur.UserId = @UserId
            ORDER BY a.AppName";
        return await connection.QueryAsync<UserRoleDto>(sql, new { UserId = userId });
    }

    public async Task<int> AssignRoleAsync(int userId, int appId)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            INSERT INTO UserRoles (UserId, AppId, CreatedAt)
            OUTPUT INSERTED.UserRoleId
            VALUES (@UserId, @AppId, GETDATE())";
        return await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, AppId = appId });
    }

    public async Task<bool> RemoveRoleAsync(int userId, int appId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "DELETE FROM UserRoles WHERE UserId = @UserId AND AppId = @AppId";
        var rows = await connection.ExecuteAsync(sql, new { UserId = userId, AppId = appId });
        return rows > 0;
    }
}
