using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAccessManager.Web.Models;
using UserAccessManager.Web.Services;

namespace UserAccessManager.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApiClientService _api;

    public HomeController(ApiClientService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var users = await _api.GetUsersAsync(1, 1);
        var apps = await _api.GetApplicationsAsync();
        var pendingRequests = await _api.GetAccessRequestsAsync(1, 1, "Pending");
        var stagingUsers = await _api.GetStagingUsersAsync(1, 1);
        var recentRequests = await _api.GetAccessRequestsAsync(1, 5);

        var model = new DashboardViewModel
        {
            TotalUsers = users.TotalCount,
            TotalApplications = apps.Count,
            PendingRequests = pendingRequests.TotalCount,
            StagingUsers = stagingUsers.TotalCount,
            RecentRequests = recentRequests.Items
        };

        return View(model);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
