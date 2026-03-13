using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserAccessManager.API.Controllers;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;
using UserAccessManager.Core.Validators;

namespace UserAccessManager.API.Tests.Controllers;

public class AccessRequestsControllerTests
{
    private readonly IAccessRequestRepository _repo = Substitute.For<IAccessRequestRepository>();
    private readonly AccessRequestsController _controller;

    public AccessRequestsControllerTests()
    {
        _controller = new AccessRequestsController(
            _repo,
            new CreateAccessRequestValidator(),
            new UpdateStatusRequestValidator());
    }

    [Fact]
    public async Task GetAllReturnsPagedResult()
    {
        var paged = new PagedResult<AccessRequestDto>
        {
            Items = [new AccessRequestDto { AccessRequestId = 1, UserName = "user1", Status = "Pending" }],
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };
        _repo.GetAllAsync(1, 20, null).Returns(paged);

        var actionResult = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<AccessRequestDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data!.Items);
    }

    [Fact]
    public async Task GetAllFiltersByStatus()
    {
        var paged = new PagedResult<AccessRequestDto> { Items = [], TotalCount = 0, Page = 1, PageSize = 20 };
        _repo.GetAllAsync(1, 20, "Approved").Returns(paged);

        var actionResult = await _controller.GetAll(status: "Approved");

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<PagedResult<AccessRequestDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Data!.Items);
    }

    [Fact]
    public async Task GetByIdWhenFoundReturnsOk()
    {
        var dto = new AccessRequestDto { AccessRequestId = 5, UserName = "user1", Status = "Pending" };
        _repo.GetByIdAsync(5).Returns(dto);

        var actionResult = await _controller.GetById(5);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestDto>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(5, response.Data!.AccessRequestId);
    }

    [Fact]
    public async Task GetByIdWhenNotFoundReturnsNotFound()
    {
        _repo.GetByIdAsync(999).Returns((AccessRequestDto?)null);

        var actionResult = await _controller.GetById(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestDto>>(notFound.Value);
        Assert.False(response.Success);
        Assert.Contains("999", response.Message);
    }

    [Fact]
    public async Task CreateWithValidRequestReturnsOk()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "user1@test.com", AppRoleId = 3 };
        var resultDto = new AccessRequestResultDto { Status = "Success", Message = "Request submitted successfully.", AppRoleId = 3, RoleName = "App3" };
        _repo.CreateAsync(request).Returns(resultDto);

        var actionResult = await _controller.Create(request);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestResultDto>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal("Success", response.Data!.Status);
    }

    [Fact]
    public async Task CreateWithMissingUserNameReturnsBadRequest()
    {
        var request = new CreateAccessRequest { UserName = "", Email = "user1@test.com", AppRoleId = 3 };

        var actionResult = await _controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestResultDto>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Contains("UserName is required.", response.Errors);
    }

    [Fact]
    public async Task CreateWithInvalidEmailReturnsBadRequest()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "not-an-email", AppRoleId = 3 };

        var actionResult = await _controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestResultDto>>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CreateWithZeroAppRoleIdReturnsBadRequest()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "user1@test.com", AppRoleId = 0 };

        var actionResult = await _controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<AccessRequestResultDto>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Contains("AppRoleId must be a positive integer.", response.Errors);
    }

    [Fact]
    public async Task UpdateStatusWithValidApprovedReturnsOk()
    {
        var updateRequest = new UpdateStatusRequest { Status = "Approved" };
        _repo.UpdateStatusAsync(1, "Approved").Returns(StatusUpdateResult.Updated("Approved"));

        var actionResult = await _controller.UpdateStatus(1, updateRequest);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.True(response.Success);
        Assert.Contains("Approved", response.Message);
    }

    [Fact]
    public async Task UpdateStatusWithFulfilledReturnsOk()
    {
        var updateRequest = new UpdateStatusRequest { Status = "Fulfilled" };
        _repo.UpdateStatusAsync(1, "Fulfilled").Returns(StatusUpdateResult.Fulfilled("App3"));

        var actionResult = await _controller.UpdateStatus(1, updateRequest);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(ok.Value);
        Assert.True(response.Success);
        Assert.Contains("fulfilled", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateStatusWhenNotFoundReturnsNotFound()
    {
        var updateRequest = new UpdateStatusRequest { Status = "Approved" };
        _repo.UpdateStatusAsync(999, "Approved").Returns(StatusUpdateResult.NotFound(999));

        var actionResult = await _controller.UpdateStatus(999, updateRequest);

        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
        Assert.False(response.Success);
        Assert.Contains("999", response.Message);
    }

    [Fact]
    public async Task UpdateStatusWhenUserInactiveReturnsNotFound()
    {
        var updateRequest = new UpdateStatusRequest { Status = "Fulfilled" };
        _repo.UpdateStatusAsync(1, "Fulfilled").Returns(StatusUpdateResult.UserInactive("user@test.com"));

        var actionResult = await _controller.UpdateStatus(1, updateRequest);

        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(notFound.Value);
        Assert.False(response.Success);
        Assert.Contains("inactive", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateStatusWithInvalidStatusReturnsBadRequest()
    {
        var updateRequest = new UpdateStatusRequest { Status = "InvalidStatus" };

        var actionResult = await _controller.UpdateStatus(1, updateRequest);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task UpdateStatusWithEmptyStatusReturnsBadRequest()
    {
        var updateRequest = new UpdateStatusRequest { Status = "" };

        var actionResult = await _controller.UpdateStatus(1, updateRequest);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Contains("Status is required.", response.Errors);
    }
}
