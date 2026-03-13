using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;

namespace UserAccessManager.Core.Interfaces;

public interface IApplicationRepository
{
    Task<IEnumerable<ApplicationDto>> GetAllAsync();
    Task<ApplicationDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateApplicationRequest request);
    Task<bool> UpdateAsync(int id, UpdateApplicationRequest request);
}
