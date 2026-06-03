using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;

namespace PropertyPal.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "PropertyManager")]
public class ReportsController : ControllerBase
{
    private readonly PropertyDbContext _context;
    public ReportsController(PropertyDbContext context) => _context = context;

    [HttpGet("occupancy")]
    public async Task<IActionResult> Occupancy()
    {
        var data = await _context.Properties.Select(p => new
        {
            p.PropertyId,
            p.Name,
            TotalUnits = p.Units.Count,
            Occupied = p.Units.Count(u => u.Status == 1),
            Available = p.Units.Count(u => u.Status == 0),
            OccupancyRate = p.Units.Count == 0 ? 0 : Math.Round((decimal)p.Units.Count(u => u.Status == 1) / p.Units.Count * 100, 1)
        }).ToListAsync();
        return Ok(data);
    }

    [HttpGet("maintenance-backlog")]
    public async Task<IActionResult> MaintenanceBacklog()
    {
        var data = await _context.MaintenanceRequests
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count(), Oldest = g.Min(r => r.CreatedAt) })
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("overdue-payments")]
    public async Task<IActionResult> OverduePayments()
    {
        var today = DateTime.Today;
        var data = await _context.Payments.Include(p => p.Lease).ThenInclude(l => l.Unit)
            .Where(p => p.Status != 1 && p.DueDate < today)
            .Select(p => new { p.PaymentId, p.Amount, p.DueDate, p.Lease.Unit.UnitNumber, p.TransactionReference })
            .ToListAsync();
        return Ok(data);
    }
}
