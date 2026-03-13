using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/staging-users")]
public class StagingUsersController : ControllerBase
{
    private readonly IStagingUserRepository _repo;

    public StagingUsersController(IStagingUserRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StagingUserDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _repo.GetAllAsync(page, pageSize);
        return Ok(ApiResponse<PagedResult<StagingUserDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StagingUserDto>>> GetById(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<StagingUserDto>.FailResponse($"Staging user with ID {id} not found."));
        return Ok(ApiResponse<StagingUserDto>.SuccessResponse(user));
    }

    [HttpPost("promote/{id:int}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Promote(int id, [FromBody] PromoteUserRequest? request)
    {
        var createdBy = request?.CreatedBy ?? "API";
        var user = await _repo.PromoteAsync(id, createdBy);
        if (user == null)
            return NotFound(ApiResponse<UserDto>.FailResponse($"Staging user with ID {id} not found."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Staging user promoted successfully."));
    }
}
