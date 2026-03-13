using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Core.Validators;

namespace UserAccessManager.API.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationRepository _repo;

    public ApplicationsController(IApplicationRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationDto>>>> GetAll()
    {
        var apps = await _repo.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ApplicationDto>>.SuccessResponse(apps));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> GetById(int id)
    {
        var app = await _repo.GetByIdAsync(id);
        if (app == null)
            return NotFound(ApiResponse<ApplicationDto>.FailResponse($"Application with ID {id} not found."));
        return Ok(ApiResponse<ApplicationDto>.SuccessResponse(app));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Create([FromBody] CreateApplicationRequest request)
    {
        var validator = new CreateApplicationRequestValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(ApiResponse<ApplicationDto>.FailResponse("Validation failed.", validation.Errors.Select(e => e.ErrorMessage).ToList()));

        var id = await _repo.CreateAsync(request);
        var app = await _repo.GetByIdAsync(id);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<ApplicationDto>.SuccessResponse(app!, "Application created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateApplicationRequest request)
    {
        var updated = await _repo.UpdateAsync(id, request);
        if (!updated)
            return NotFound(ApiResponse<object>.FailResponse($"Application with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Application updated successfully."));
    }
}
