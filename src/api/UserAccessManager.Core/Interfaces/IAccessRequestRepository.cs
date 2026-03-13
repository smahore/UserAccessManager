using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IAccessRequestRepository
{
    Task<PagedResult<AccessRequestDto>> GetAllAsync(int page, int pageSize, string? status = null);
    Task<AccessRequestDto?> GetByIdAsync(int id);
    Task<AccessRequestResultDto> CreateAsync(CreateAccessRequest request);
    Task<bool> UpdateStatusAsync(int id, string status);
}
