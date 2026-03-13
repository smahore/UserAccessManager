using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Web.Models;
using UserAccessManager.Web.Services;

namespace UserAccessManager.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly ApiClientService _api;

    public UsersController(ApiClientService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var result = await _api.GetUsersAsync(page, 20, search);
        ViewBag.Search = search;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _api.GetUserByIdAsync(id);
        if (user is null)
            return NotFound();

        var roles = await _api.GetUserRolesAsync(id);
        var apps = await _api.GetApplicationsAsync();
        ViewBag.Roles = roles;
        ViewBag.Applications = apps;
        return View(user);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var response = await _api.CreateUserAsync(request);
        if (response.Success)
        {
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
            ModelState.AddModelError(string.Empty, error);
        if (!string.IsNullOrEmpty(response.Message))
            ModelState.AddModelError(string.Empty, response.Message);
        return View(request);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _api.GetUserByIdAsync(id);
        if (user is null)
            return NotFound();

        var model = new UpdateUserRequest
        {
            FullName = user.FullName,
            Email = user.Email,
            Source = user.Source
        };
        ViewBag.UserId = id;
        ViewBag.UserName = user.UserName;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            return View(request);
        }

        var response = await _api.UpdateUserAsync(id, request);
        if (response.Success)
        {
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        foreach (var error in response.Errors)
            ModelState.AddModelError(string.Empty, error);
        ViewBag.UserId = id;
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id, bool isActive)
    {
        var response = await _api.UpdateUserStatusAsync(id, !isActive);
        TempData[response.Success ? "Success" : "Error"] = response.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(int id, int appId)
    {
        var response = await _api.AssignRoleAsync(id, appId);
        TempData[response.Success ? "Success" : "Error"] = response.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(int id, int appId)
    {
        var response = await _api.RemoveRoleAsync(id, appId);
        TempData[response.Success ? "Success" : "Error"] = response.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
}
