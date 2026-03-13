using Dapper;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class AccessRequestRepository : IAccessRequestRepository
{
    private readonly DapperContext _context;

    public AccessRequestRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AccessRequestDto>> GetAllAsync(int page, int pageSize, string? status = null)
    {
        using var connection = _context.CreateConnection();

        var offset = (page - 1) * pageSize;
        var whereClause = string.IsNullOrWhiteSpace(status) ? string.Empty : "WHERE Status = @Status";

        var countSql = $"SELECT COUNT(*) FROM ApplicationAccessRequests {whereClause}";
        var dataSql = $@"
            SELECT AccessRequestId, UserName, RoleName, Status, CreatedDate, AppRoleId, Email
            FROM ApplicationAccessRequests
            {whereClause}
            ORDER BY CreatedDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new { Status = status, Offset = offset, PageSize = pageSize };

        var total = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<AccessRequestDto>(dataSql, parameters);

        return new PagedResult<AccessRequestDto>
        {
            Items = items.ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AccessRequestDto?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            SELECT AccessRequestId, UserName, RoleName, Status, CreatedDate, AppRoleId, Email
            FROM ApplicationAccessRequests
            WHERE AccessRequestId = @AccessRequestId";
        return await connection.QuerySingleOrDefaultAsync<AccessRequestDto>(sql, new { AccessRequestId = id });
    }

    public async Task<AccessRequestResultDto> CreateAsync(CreateAccessRequest request)
    {
        using var connection = _context.CreateConnection();
        const string sql = "EXEC ManageApplicationAccessRequests @UserName, @Email, @AppRoleId";
        var result = await connection.QuerySingleOrDefaultAsync<AccessRequestResultDto>(sql, new
        {
            request.UserName,
            request.Email,
            request.AppRoleId
        });
        return result ?? new AccessRequestResultDto { Status = "Error", Message = "No result returned from stored procedure." };
    }

    public async Task<StatusUpdateResult> UpdateStatusAsync(int id, string status)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        // Look up the access request
        const string selectSql = @"
            SELECT AccessRequestId, UserName, RoleName, Status, CreatedDate, AppRoleId, Email
            FROM ApplicationAccessRequests
            WHERE AccessRequestId = @AccessRequestId";
        var accessRequest = await connection.QuerySingleOrDefaultAsync<AccessRequestDto>(
            selectSql, new { AccessRequestId = id }, transaction);

        if (accessRequest is null)
            return StatusUpdateResult.NotFound(id);

        if (string.Equals(status, "Fulfilled", StringComparison.OrdinalIgnoreCase))
        {
            // Find the user by email
            const string userSql = @"
                SELECT UserId, IsActive
                FROM Users
                WHERE Email = @Email";
            var user = await connection.QuerySingleOrDefaultAsync<dynamic>(
                userSql, new { accessRequest.Email }, transaction);

            if (user is null)
                return StatusUpdateResult.UserNotFound(accessRequest.Email ?? accessRequest.UserName);

            if (!(bool)user.IsActive)
                return StatusUpdateResult.UserInactive(accessRequest.Email ?? accessRequest.UserName);

            int userId = (int)user.UserId;

            // Check if the role is already assigned
            const string existsSql = @"
                SELECT COUNT(1) FROM UserRoles
                WHERE UserId = @UserId AND AppId = @AppId";
            var alreadyAssigned = await connection.ExecuteScalarAsync<int>(
                existsSql, new { UserId = userId, AppId = accessRequest.AppRoleId }, transaction);

            if (alreadyAssigned > 0)
                return StatusUpdateResult.RoleAlreadyAssigned(accessRequest.RoleName);

            // Grant the role
            const string insertRoleSql = @"
                INSERT INTO UserRoles (UserId, AppId, CreatedAt)
                VALUES (@UserId, @AppId, GETDATE())";
            await connection.ExecuteAsync(
                insertRoleSql, new { UserId = userId, AppId = accessRequest.AppRoleId }, transaction);
        }

        // Update the status
        const string updateSql = @"
            UPDATE ApplicationAccessRequests
            SET Status = @Status
            WHERE AccessRequestId = @AccessRequestId";
        await connection.ExecuteAsync(
            updateSql, new { Status = status, AccessRequestId = id }, transaction);

        transaction.Commit();

        return string.Equals(status, "Fulfilled", StringComparison.OrdinalIgnoreCase)
            ? StatusUpdateResult.Fulfilled(accessRequest.RoleName)
            : StatusUpdateResult.Updated(status);
    }
}
