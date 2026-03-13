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

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE ApplicationAccessRequests
            SET Status = @Status
            WHERE AccessRequestId = @AccessRequestId";
        var rows = await connection.ExecuteAsync(sql, new { Status = status, AccessRequestId = id });
        return rows > 0;
    }
}
