using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Web.Models;
using UserAccessManager.Web.Services;

namespace UserAccessManager.Web.Controllers;

public class AccessRequestsController : Controller
{
    private readonly ApiClientService _api;

    public AccessRequestsController(ApiClientService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? status = null)
    {
        var result = await _api.GetAccessRequestsAsync(page, 20, status);
        ViewBag.StatusFilter = status;
        return View(result);
    }

    public async Task<IActionResult> Create()
    {
        var apps = await _api.GetApplicationsAsync();
        ViewBag.Applications = apps;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccessRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Applications = await _api.GetApplicationsAsync();
            return View(request);
        }

        var response = await _api.CreateAccessRequestAsync(request);
        if (response.Success)
        {
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
            ModelState.AddModelError(string.Empty, error);
        if (!string.IsNullOrEmpty(response.Message))
            ModelState.AddModelError(string.Empty, response.Message);
        ViewBag.Applications = await _api.GetApplicationsAsync();
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var response = await _api.UpdateAccessRequestStatusAsync(id, status);
        TempData[response.Success ? "Success" : "Error"] = response.Message;
        return RedirectToAction(nameof(Index));
    }
}
