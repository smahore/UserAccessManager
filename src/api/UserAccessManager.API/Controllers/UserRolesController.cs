using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/users/{userId:int}/roles")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleRepository _roleRepo;
    private readonly IUserRepository _userRepo;

    public UserRolesController(IUserRoleRepository roleRepo, IUserRepository userRepo)
    {
        _roleRepo = roleRepo;
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleDto>>>> GetRoles(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return NotFound(ApiResponse<IEnumerable<UserRoleDto>>.FailResponse($"User with ID {userId} not found."));

        var roles = await _roleRepo.GetByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<UserRoleDto>>.SuccessResponse(roles));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserRoleDto>>> AssignRole(int userId, [FromBody] AssignRoleRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return NotFound(ApiResponse<UserRoleDto>.FailResponse($"User with ID {userId} not found."));

        var roleId = await _roleRepo.AssignRoleAsync(userId, request.AppId);
        var roles = await _roleRepo.GetByUserIdAsync(userId);
        var assigned = roles.FirstOrDefault(r => r.UserRoleId == roleId);
        return Ok(ApiResponse<UserRoleDto>.SuccessResponse(assigned!, "Role assigned successfully."));
    }

    [HttpDelete("{appId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveRole(int userId, int appId)
    {
        var removed = await _roleRepo.RemoveRoleAsync(userId, appId);
        if (!removed)
            return NotFound(ApiResponse<object>.FailResponse($"Role assignment not found for user {userId} and app {appId}."));
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Role removed successfully."));
    }
}
