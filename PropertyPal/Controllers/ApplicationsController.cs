using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;
using AppModel = PropertyPal.Api.Models.Application;

namespace PropertyPal.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly PropertyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public ApplicationsController(PropertyDbContext context, UserManager<ApplicationUser> userManager) { _context = context; _userManager = userManager; }

    // Shows the application form for an available unit. Only tenants can apply.
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Apply(int unitId)
    {
        var unit = await _context.Units.Include(u => u.Property).FirstOrDefaultAsync(u => u.UnitId == unitId && u.Status == 0);
        if (unit == null) return NotFound();
        return View(unit);
    }

    // Saves a tenant application after checking the unit is still vacant.
    [HttpPost, Authorize(Roles = "Tenant"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyConfirmed(int unitId)
    {
        var tenantId = _userManager.GetUserId(User)!;
        var unit = await _context.Units.FindAsync(unitId);
        if (unit == null || unit.Status != 0) return BadRequest("Unit is not available.");
        _context.Applications.Add(new AppModel { TenantId = tenantId, UnitId = unitId, Status = 0, CreatedAt = DateTime.Now });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Application submitted.";
        return RedirectToAction(nameof(My));
    }

    // Lists the current tenant applications so tenants can track their request status.
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> My()
    {
        var tenantId = _userManager.GetUserId(User)!;
        return View(await _context.Applications.Include(a => a.Unit).ThenInclude(u => u.Property).Where(a => a.TenantId == tenantId).OrderByDescending(a => a.CreatedAt).ToListAsync());
    }

    // Manager view: shows all applications for approval/rejection.
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Index() => View(await _context.Applications.Include(a => a.Unit).ThenInclude(u => u.Property).OrderByDescending(a => a.CreatedAt).ToListAsync());

    // Updates an application decision. When approved, the code also marks the unit occupied and creates a one-year lease.
    [HttpPost, Authorize(Roles = "PropertyManager"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, int status)
    {
        var app = await _context.Applications.Include(a => a.Unit).FirstOrDefaultAsync(a => a.ApplicationId == id);
        if (app == null) return NotFound();
        if (status == 2 && await _context.Leases.AnyAsync(l => l.UnitId == app.UnitId && l.IsActive))
        {
            TempData["Error"] = "This unit already has an active lease.";
            return RedirectToAction(nameof(Index));
        }
        app.Status = status;
        if (status == 2)
        {
            app.Unit.Status = 1;
            _context.Leases.Add(new Lease { TenantId = app.TenantId, UnitId = app.UnitId, StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1), IsActive = true });
        }
        await _context.SaveChangesAsync();
        TempData["Success"] = "Application updated.";
        return RedirectToAction(nameof(Index));
    }
}
