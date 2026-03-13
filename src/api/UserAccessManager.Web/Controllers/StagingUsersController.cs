using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Web.Services;

namespace UserAccessManager.Web.Controllers;

[Authorize]
public class StagingUsersController : Controller
{
    private readonly ApiClientService _api;

    public StagingUsersController(ApiClientService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _api.GetStagingUsersAsync(page, 20);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote(int id)
    {
        var response = await _api.PromoteStagingUserAsync(id, "WebUI");
        TempData[response.Success ? "Success" : "Error"] = response.Message;
        return RedirectToAction(nameof(Index));
    }
}
