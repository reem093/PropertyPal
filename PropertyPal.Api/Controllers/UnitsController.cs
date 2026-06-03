using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;

namespace PropertyPal.Api.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly PropertyDbContext _context;
    public UnitsController(PropertyDbContext context) => _context = context;

    [HttpGet("vacant")]
    [AllowAnonymous]
    public async Task<IActionResult> VacantUnits() => Ok(await _context.Units.Include(u => u.Property)
        .Where(u => u.Status == 0)
        .Select(u => new { u.UnitId, u.UnitNumber, u.RentAmount, u.Size, Type = u.Type, Property = u.Property.Name, u.Property.Location })
        .ToListAsync());

    [HttpGet]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> All() => Ok(await _context.Units.Include(u => u.Property).ToListAsync());
}
