using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/access-requests")]
public class AccessRequestsController : ControllerBase
{
    private readonly IAccessRequestRepository _repo;
    private readonly IValidator<CreateAccessRequest> _createValidator;
    private readonly IValidator<UpdateStatusRequest> _statusValidator;

    public AccessRequestsController(
        IAccessRequestRepository repo,
        IValidator<CreateAccessRequest> createValidator,
        IValidator<UpdateStatusRequest> statusValidator)
    {
        _repo = repo;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AccessRequestDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var result = await _repo.GetAllAsync(page, pageSize, status);
        return Ok(ApiResponse<PagedResult<AccessRequestDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AccessRequestDto>>> GetById(int id)
    {
        var request = await _repo.GetByIdAsync(id);
        if (request == null)
            return NotFound(ApiResponse<AccessRequestDto>.FailResponse($"Access request with ID {id} not found."));
        return Ok(ApiResponse<AccessRequestDto>.SuccessResponse(request));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AccessRequestResultDto>>> Create([FromBody] CreateAccessRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<AccessRequestResultDto>.FailResponse("Validation failed.", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var result = await _repo.CreateAsync(request);
        return Ok(ApiResponse<AccessRequestResultDto>.SuccessResponse(result, result.Message));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var validation = await _statusValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<object>.FailResponse("Validation failed.", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var result = await _repo.UpdateStatusAsync(id, request.Status);
        if (!result.Success)
            return NotFound(ApiResponse<object>.FailResponse(result.Message));
        return Ok(ApiResponse<object>.SuccessResponse(new { }, result.Message));
    }
}
