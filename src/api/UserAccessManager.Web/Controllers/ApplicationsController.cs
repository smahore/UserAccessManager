using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Web.Models;
using UserAccessManager.Web.Services;

namespace UserAccessManager.Web.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly ApiClientService _api;

    public ApplicationsController(ApiClientService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var apps = await _api.GetApplicationsAsync();
        return View(apps);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateApplicationRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var response = await _api.CreateApplicationAsync(request);
        if (response.Success)
        {
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
            ModelState.AddModelError(string.Empty, error);
        return View(request);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var app = await _api.GetApplicationByIdAsync(id);
        if (app is null)
            return NotFound();

        var model = new UpdateApplicationRequest
        {
            AppName = app.AppName,
            Description = app.Description
        };
        ViewBag.AppId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateApplicationRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AppId = id;
            return View(request);
        }

        var response = await _api.UpdateApplicationAsync(id, request);
        if (response.Success)
        {
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in response.Errors)
            ModelState.AddModelError(string.Empty, error);
        ViewBag.AppId = id;
        return View(request);
    }
}
