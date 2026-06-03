using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;
using PropertyPal.Api.Models;

namespace PropertyPal.Controllers;

public class UnitsController : Controller
{
    private readonly PropertyDbContext _context;
    public UnitsController(PropertyDbContext context) => _context = context;

    // Public page: shows vacant units to visitors before they log in.
    [AllowAnonymous]
    public async Task<IActionResult> Vacant() => View(await _context.Units.Include(u => u.Property).Where(u => u.Status == 0).ToListAsync());

    // Manager page: lists all units with property details for administration.
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Index() => View(await _context.Units.Include(u => u.Property).ToListAsync());

    // Loads the create-unit form and provides the property dropdown list.
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Properties = await _context.Properties.ToListAsync();
        return View(new Unit { Status = 0, Type = 1 });
    }

    // Validates and saves a new unit entered by the property manager.
    [HttpPost, Authorize(Roles = "PropertyManager"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Unit unit)
    {
        if (!ModelState.IsValid) { ViewBag.Properties = await _context.Properties.ToListAsync(); return View(unit); }
        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Unit created.";
        return RedirectToAction(nameof(Index));
    }
}
