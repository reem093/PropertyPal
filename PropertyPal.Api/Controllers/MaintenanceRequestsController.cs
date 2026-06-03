using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;

namespace PropertyPal.Api.Controllers;

[ApiController]
[Route("api/maintenance-requests")]
[Authorize]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly PropertyDbContext _context;
    public MaintenanceRequestsController(PropertyDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "PropertyManager,MaintenanceStaff")]
    public async Task<IActionResult> GetRequests() => Ok(await _context.MaintenanceRequests.Include(r => r.Unit).OrderByDescending(r => r.CreatedAt).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRequest(int id)
    {
        var request = await _context.MaintenanceRequests.Include(r => r.Unit).Include(r => r.MaintenanceUpdates).FirstOrDefaultAsync(r => r.RequestId == id);
        return request == null ? NotFound() : Ok(request);
    }
}
