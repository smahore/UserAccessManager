using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserAccessManager.API.Controllers;
using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.DTOs.Response;
using UserAccessManager.Core.Interfaces;

namespace UserAccessManager.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IAuthRepository _repo = Substitute.For<IAuthRepository>();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_repo);
    }

    [Fact]
    public async Task GetUserRolesWithValidRequestReturnsRoles()
    {
        var request = new UserRolesLookupRequest { UserName = "testuser", AllowedRoles = "App1,App2" };
        var roles = new List<UserRoleDto>
        {
            new() { UserId = 1, AppName = "App1", AppId = 1 },
            new() { UserId = 1, AppName = "App2", AppId = 2 }
        };
        _repo.GetUserRolesByUserNameAsync(request).Returns(roles);

        var actionResult = await _controller.GetUserRoles(request);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<UserRoleDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(2, response.Data!.Count());
    }

    [Fact]
    public async Task GetUserRolesWithEmptyUserNameReturnsBadRequest()
    {
        var request = new UserRolesLookupRequest { UserName = "", AllowedRoles = "App1" };

        var actionResult = await _controller.GetUserRoles(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<UserRoleDto>>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Equal("UserName is required.", response.Message);
    }

    [Fact]
    public async Task GetUserRolesWithWhitespaceUserNameReturnsBadRequest()
    {
        var request = new UserRolesLookupRequest { UserName = "   ", AllowedRoles = "App1" };

        var actionResult = await _controller.GetUserRoles(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<UserRoleDto>>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Equal("UserName is required.", response.Message);
    }

    [Fact]
    public async Task GetUserRolesWithEmptyAllowedRolesReturnsBadRequest()
    {
        var request = new UserRolesLookupRequest { UserName = "testuser", AllowedRoles = "" };

        var actionResult = await _controller.GetUserRoles(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<UserRoleDto>>>(badRequest.Value);
        Assert.False(response.Success);
        Assert.Equal("AllowedRoles is required.", response.Message);
    }

    [Fact]
    public async Task GetUserRolesWhenNoMatchesReturnsEmptyList()
    {
        var request = new UserRolesLookupRequest { UserName = "unknownuser", AllowedRoles = "App1" };
        _repo.GetUserRolesByUserNameAsync(request).Returns(Enumerable.Empty<UserRoleDto>());

        var actionResult = await _controller.GetUserRoles(request);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<UserRoleDto>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Data!);
    }
}
