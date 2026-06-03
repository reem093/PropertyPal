using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Data;

namespace PropertyPal.Api.Controllers;

[ApiController]
[Route("api/public/maintenance")]
public class PublicMaintenanceController : ControllerBase
{
    private readonly PropertyDbContext _context;
    public PublicMaintenanceController(PropertyDbContext context) => _context = context;

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string ticketNumber, [FromQuery] string phone)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber) || string.IsNullOrWhiteSpace(phone)) return BadRequest("Ticket number and phone are required.");
        var request = await _context.MaintenanceRequests
            .Include(r => r.Unit).ThenInclude(u => u.Property)
            .Include(r => r.MaintenanceUpdates)
            .FirstOrDefaultAsync(r => r.TicketNumber == ticketNumber);
        if (request == null) return NotFound();
        var tenant = await _context.Users.FindAsync(request.TenantId);
        if (tenant?.PhoneNumber != phone) return NotFound();
        return Ok(new
        {
            request.TicketNumber,
            request.Title,
            request.Description,
            Status = StatusText(request.Status),
            Priority = PriorityText(request.Priority),
            Category = CategoryText(request.Category),
            request.CreatedAt,
            request.CompletedAt,
            Unit = request.Unit.UnitNumber,
            Property = request.Unit.Property.Name,
            History = request.MaintenanceUpdates.OrderByDescending(u => u.CreatedAt).Select(u => new { u.CreatedAt, u.Notes })
        });
    }

    private static string StatusText(int value) => value switch { 0 => "Submitted", 1 => "Assigned", 2 => "In Progress", 3 => "Resolved", 4 => "Closed", _ => "Unknown" };
    private static string PriorityText(int value) => value switch { 1 => "Low", 2 => "Medium", 3 => "High", 4 => "Emergency", _ => "Unknown" };
    private static string CategoryText(int value) => value switch { 1 => "Plumbing", 2 => "Electrical", 3 => "HVAC", 4 => "Carpentry", _ => "General" };
}
