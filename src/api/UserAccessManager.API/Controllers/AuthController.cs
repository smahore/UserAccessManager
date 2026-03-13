using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repo;

    public AuthController(IAuthRepository repo) => _repo = repo;

    [HttpPost("user-roles")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleDto>>>> GetUserRoles([FromBody] UserRolesLookupRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            return BadRequest(ApiResponse<IEnumerable<UserRoleDto>>.FailResponse("UserName is required."));
        if (string.IsNullOrWhiteSpace(request.AllowedRoles))
            return BadRequest(ApiResponse<IEnumerable<UserRoleDto>>.FailResponse("AllowedRoles is required."));

        var roles = await _repo.GetUserRolesByUserNameAsync(request);
        return Ok(ApiResponse<IEnumerable<UserRoleDto>>.SuccessResponse(roles));
    }
}
