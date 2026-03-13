using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Core.Validators;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var result = await _repo.GetAllAsync(page, pageSize, search);
        return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<UserDto>.FailResponse($"User with ID {id} not found."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetByUsername(string username)
    {
        var user = await _repo.GetByUsernameAsync(username);
        if (user == null)
            return NotFound(ApiResponse<UserDto>.FailResponse($"User '{username}' not found."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserRequest request)
    {
        var validator = new CreateUserRequestValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<UserDto>.FailResponse("Validation failed.", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var id = await _repo.CreateAsync(request);
        var user = await _repo.GetByIdAsync(id);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<UserDto>.SuccessResponse(user!, "User created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var validator = new UpdateUserRequestValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.FailResponse("Validation failed.", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var updated = await _repo.UpdateAsync(id, request);
        if (!updated)
            return NotFound(ApiResponse<object>.FailResponse($"User with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "User updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(int id, [FromBody] bool isActive)
    {
        var updated = await _repo.UpdateStatusAsync(id, isActive);
        if (!updated)
            return NotFound(ApiResponse<object>.FailResponse($"User with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(new { }, $"User {(isActive ? "activated" : "deactivated")} successfully."));
    }
}
