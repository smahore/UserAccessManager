using Dapper;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, string? search = null)
    {
        using var connection = _context.CreateConnection();

        var offset = (page - 1) * pageSize;
        var whereClause = string.IsNullOrWhiteSpace(search)
            ? string.Empty
            : "WHERE UserName LIKE @Search OR FullName LIKE @Search OR Email LIKE @Search";

        var countSql = $"SELECT COUNT(*) FROM Users {whereClause}";
        var dataSql = $@"
            SELECT UserId, UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Users
            {whereClause}
            ORDER BY UserId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new
        {
            Search = $"%{search}%",
            Offset = offset,
            PageSize = pageSize
        };

        var total = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<UserDto>(dataSql, parameters);

        return new PagedResult<UserDto>
        {
            Items = items.ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            SELECT UserId, UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Users
            WHERE UserId = @UserId";
        return await connection.QuerySingleOrDefaultAsync<UserDto>(sql, new { UserId = id });
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            SELECT UserId, UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Users
            WHERE UserName = @UserName";
        return await connection.QuerySingleOrDefaultAsync<UserDto>(sql, new { UserName = username });
    }

    public async Task<int> CreateAsync(CreateUserRequest request)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            INSERT INTO Users (UserName, FullName, Email, Source, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
            OUTPUT INSERTED.UserId
            VALUES (@UserName, @FullName, @Email, @Source, 1, GETDATE(), GETDATE(), @CreatedBy, @CreatedBy)";
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            request.UserName,
            request.FullName,
            request.Email,
            request.Source,
            request.CreatedBy
        });
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserRequest request)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE Users
            SET FullName   = COALESCE(@FullName, FullName),
                Email      = COALESCE(@Email, Email),
                Source     = COALESCE(@Source, Source),
                UpdatedAt  = GETDATE(),
                UpdatedBy  = @UpdatedBy
            WHERE UserId = @UserId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            request.FullName,
            request.Email,
            request.Source,
            request.UpdatedBy,
            UserId = id
        });
        return rows > 0;
    }

    public async Task<bool> UpdateStatusAsync(int id, bool isActive)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE Users SET IsActive = @IsActive, UpdatedAt = GETDATE()
            WHERE UserId = @UserId";
        var rows = await connection.ExecuteAsync(sql, new { IsActive = isActive, UserId = id });
        return rows > 0;
    }
}
