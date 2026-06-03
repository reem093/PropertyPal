using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PropertyPal.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("PropertyManager")) return RedirectToAction("Manager", "Dashboard");
        if (User.IsInRole("MaintenanceStaff")) return RedirectToAction("Staff", "Dashboard");
        return RedirectToAction("Tenant", "Dashboard");
    }

    [Authorize(Roles = "PropertyManager")] public IActionResult Manager() => View();
    [Authorize(Roles = "Tenant")] public IActionResult Tenant() => View();
    [Authorize(Roles = "MaintenanceStaff")] public IActionResult Staff() => View();
}
