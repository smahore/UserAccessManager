using Dapper;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Infrastructure.Data;

namespace UserAccessManager.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly DapperContext _context;

    public ApplicationRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT AppId, AppName, Description, CreatedAt FROM ApplicationName ORDER BY AppName";
        return await connection.QueryAsync<ApplicationDto>(sql);
    }

    public async Task<ApplicationDto?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT AppId, AppName, Description, CreatedAt FROM ApplicationName WHERE AppId = @AppId";
        return await connection.QuerySingleOrDefaultAsync<ApplicationDto>(sql, new { AppId = id });
    }

    public async Task<int> CreateAsync(CreateApplicationRequest request)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            INSERT INTO ApplicationName (AppName, Description, CreatedAt)
            OUTPUT INSERTED.AppId
            VALUES (@AppName, @Description, GETDATE())";
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            request.AppName,
            request.Description
        });
    }

    public async Task<bool> UpdateAsync(int id, UpdateApplicationRequest request)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE ApplicationName
            SET AppName     = COALESCE(@AppName, AppName),
                Description = COALESCE(@Description, Description)
            WHERE AppId = @AppId";
        var rows = await connection.ExecuteAsync(sql, new
        {
            request.AppName,
            request.Description,
            AppId = id
        });
        return rows > 0;
    }
}
