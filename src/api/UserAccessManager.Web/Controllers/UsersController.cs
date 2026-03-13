using ClosedXML.Excel;
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

    public async Task<IActionResult> Create()
    {
        var stagingUsers = await _api.GetAllStagingUsersAsync();
        ViewBag.StagingUsers = stagingUsers;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.StagingUsers = await _api.GetAllStagingUsersAsync();
            return View(request);
        }

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
        ViewBag.StagingUsers = await _api.GetAllStagingUsersAsync();
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

    // ── Export ──────────────────────────────────────────────────────────

    public async Task<IActionResult> Export()
    {
        var users = await _api.GetAllUsersAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Users");

        ws.Cell(1, 1).Value = "UserName";
        ws.Cell(1, 2).Value = "FullName";
        ws.Cell(1, 3).Value = "Email";
        ws.Cell(1, 4).Value = "Source";
        ws.Cell(1, 5).Value = "IsActive";
        ws.Cell(1, 6).Value = "CreatedAt";

        var headerRange = ws.Range(1, 1, 1, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#e2e8f0");

        for (int i = 0; i < users.Count; i++)
        {
            var u = users[i];
            int row = i + 2;
            ws.Cell(row, 1).Value = u.UserName;
            ws.Cell(row, 2).Value = u.FullName ?? "";
            ws.Cell(row, 3).Value = u.Email ?? "";
            ws.Cell(row, 4).Value = u.Source;
            ws.Cell(row, 5).Value = u.IsActive ? "Active" : "Inactive";
            ws.Cell(row, 6).Value = u.CreatedAt.ToString("yyyy-MM-dd");
        }

        ws.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
