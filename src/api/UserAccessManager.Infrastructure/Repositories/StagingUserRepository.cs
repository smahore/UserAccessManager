using Dapper;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class StagingUserRepository : IStagingUserRepository
{
    private readonly DapperContext _context;

    public StagingUserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<StagingUserDto>> GetAllAsync(int page, int pageSize)
    {
        using var connection = _context.CreateConnection();

        var offset = (page - 1) * pageSize;
        const string countSql = "SELECT COUNT(*) FROM StaggingUsers";
        const string dataSql = @"
            SELECT UserId, UserName, FullName, Email, Source
            FROM StaggingUsers
            ORDER BY UserId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var total = await connection.ExecuteScalarAsync<int>(countSql);
        var items = await connection.QueryAsync<StagingUserDto>(dataSql, new { Offset = offset, PageSize = pageSize });

        return new PagedResult<StagingUserDto>
        {
            Items = items.AsList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<StagingUserDto?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            SELECT UserId, UserName, FullName, Email, Source
            FROM StaggingUsers
            WHERE UserId = @UserId";
        return await connection.QuerySingleOrDefaultAsync<StagingUserDto>(sql, new { UserId = id });
    }

    public async Task<UserDto?> PromoteAsync(int stagingUserId, string createdBy)
    {
        var staging = await GetByIdAsync(stagingUserId);
        if (staging == null)
            return null;

        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string insertSql = @"
                INSERT INTO Users (UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
                OUTPUT INSERTED.UserId
                VALUES (@UserName, @FullName, @Email, @Source, 1, GETDATE(), GETDATE(), @CreatedBy, @CreatedBy)";

            var newUserId = await connection.ExecuteScalarAsync<int>(insertSql, new
            {
                staging.UserName,
                staging.FullName,
                staging.Email,
                staging.Source,
                CreatedBy = createdBy
            }, transaction);

            const string deleteSql = "DELETE FROM StaggingUsers WHERE UserId = @UserId";
            await connection.ExecuteAsync(deleteSql, new { UserId = stagingUserId }, transaction);

            transaction.Commit();

            const string selectSql = @"
                SELECT UserId, UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
                FROM Users WHERE UserId = @UserId";
            return await connection.QuerySingleOrDefaultAsync<UserDto>(selectSql, new { UserId = newUserId });
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
